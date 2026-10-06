using System.Diagnostics;
using System.Net.Http;

namespace AddressManager.Api.Infrastructure;

/// <summary>
/// Запускает Angular dev-сервер (npm start) автоматически при старте API в Development.
/// Останавливает его при остановке приложения.
/// </summary>
public sealed class AngularDevServerService : IHostedService, IDisposable
{
    private const string AngularUrl = "http://localhost:4200";
    private readonly ILogger<AngularDevServerService> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private Process? _process;
    private CancellationTokenSource? _readinessCancellation;

    public AngularDevServerService(
        ILogger<AngularDevServerService> logger,
        IWebHostEnvironment env,
        IHostApplicationLifetime applicationLifetime)
    {
        _logger = logger;
        _env = env;
        _applicationLifetime = applicationLifetime;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Путь к AddressManager.Client относительно папки API
        var clientPath = Path.GetFullPath(
            Path.Combine(_env.ContentRootPath, "..", "AddressManager.Client"));

        if (!Directory.Exists(clientPath))
        {
            _logger.LogWarning("Angular client folder not found at {Path}. Skipping.", clientPath);
            return Task.CompletedTask;
        }

        _logger.LogInformation("Starting Angular dev server at {Path}...", clientPath);

        _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/d /s /c \"if not exist node_modules (npm ci) && npm start\"",
                WorkingDirectory = clientPath,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };

        try
        {
            _process.Start();
            _process.OutputDataReceived += (_, e) => LogAngularOutput(e.Data, LogLevel.Information);
            _process.ErrorDataReceived += (_, e) => LogAngularOutput(e.Data, LogLevel.Warning);
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
            _readinessCancellation = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                _applicationLifetime.ApplicationStopping);
            _ = WaitForAngularAndOpenBrowserAsync(_readinessCancellation.Token);
            _logger.LogInformation(
                "Angular dev server started (PID {Pid}). The browser will open when {Url} is ready.",
                _process.Id,
                AngularUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start Angular dev server.");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _readinessCancellation?.Cancel();

        if (_process is { HasExited: false })
        {
            _logger.LogInformation("Stopping Angular dev server (PID {Pid})...", _process.Id);
            try { _process.Kill(entireProcessTree: true); }
            catch (Exception ex) { _logger.LogWarning(ex, "Could not kill Angular process."); }
        }
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _readinessCancellation?.Dispose();
        _process?.Dispose();
    }

    private void LogAngularOutput(string? line, LogLevel level)
    {
        if (!string.IsNullOrWhiteSpace(line))
        {
            _logger.Log(level, "Angular: {Message}", line);
        }
    }

    private async Task WaitForAngularAndOpenBrowserAsync(CancellationToken cancellationToken)
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(1) };

        for (var attempt = 0; attempt < 90 && !cancellationToken.IsCancellationRequested; attempt++)
        {
            try
            {
                using var response = await client.GetAsync(AngularUrl, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Angular is ready at {Url}; opening the browser.", AngularUrl);
                    Process.Start(new ProcessStartInfo(AngularUrl) { UseShellExecute = true });
                    return;
                }
            }
            catch (HttpRequestException)
            {
                // Angular is still compiling.
            }
            catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }

        if (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Angular did not become ready at {Url}. Check the API debug output for npm errors.", AngularUrl);
        }
    }
}
