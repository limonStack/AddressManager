using System.Diagnostics;

namespace AddressManager.Api.Infrastructure;

/// <summary>
/// Запускает Angular dev-сервер (npm start) автоматически при старте API в Development.
/// Останавливает его при остановке приложения.
/// </summary>
public sealed class AngularDevServerService : IHostedService, IDisposable
{
    private readonly ILogger<AngularDevServerService> _logger;
    private readonly IWebHostEnvironment _env;
    private Process? _process;

    public AngularDevServerService(
        ILogger<AngularDevServerService> logger,
        IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Путь к AddressManager.Client относительно папки API
        var clientPath = Path.GetFullPath(
            Path.Combine(_env.ContentRootPath, "..", "..", "AddressManager.Client"));

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
                FileName               = "cmd.exe",
                Arguments              = "/c npm start",
                WorkingDirectory       = clientPath,
                UseShellExecute        = true,
                CreateNoWindow         = false,
                WindowStyle            = ProcessWindowStyle.Minimized
            }
        };

        try
        {
            _process.Start();
            _logger.LogInformation(
                "Angular dev server started (PID {Pid}). Open http://localhost:4200",
                _process.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start Angular dev server.");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (_process is { HasExited: false })
        {
            _logger.LogInformation("Stopping Angular dev server (PID {Pid})...", _process.Id);
            try { _process.Kill(entireProcessTree: true); }
            catch (Exception ex) { _logger.LogWarning(ex, "Could not kill Angular process."); }
        }
        return Task.CompletedTask;
    }

    public void Dispose() => _process?.Dispose();
}
