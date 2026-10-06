// ============================================================
// AngularDevServerService — фоновый сервис автозапуска Angular
//
// Реализует IHostedService, поэтому ASP.NET Core запускает его
// вместе с приложением и останавливает при завершении.
//
// Логика работы:
//   1. StartAsync  — запускает `npm start` в папке AddressManager.Client
//   2. Фоновая задача WaitForAngularAndOpenBrowserAsync — каждую секунду
//      опрашивает http://localhost:4200, и открывает браузер, как только
//      Angular ответит 200 OK (сигнал: компиляция завершена).
//   3. StopAsync   — убивает дочерний процесс вместе со всем деревом
//      (cmd → node), чтобы порт 4200 освободился.
// ============================================================

using System.Diagnostics;
using System.Net.Http;

namespace AddressManager.Api.Infrastructure;

/// <summary>
/// Запускает Angular dev-сервер (npm start) автоматически при старте API в Development.
/// Останавливает его при остановке приложения.
/// </summary>
public sealed class AngularDevServerService : IHostedService, IDisposable
{
    // URL Angular dev-сервера — используется и для опроса готовности, и для открытия браузера
    private const string AngularUrl = "http://localhost:4200";

    private readonly ILogger<AngularDevServerService> _logger;

    // IWebHostEnvironment нужен для получения ContentRootPath (папка API),
    // от которой отсчитывается относительный путь к клиенту
    private readonly IWebHostEnvironment _env;

    // IHostApplicationLifetime позволяет подписаться на событие остановки приложения
    // и отменить ожидание Angular, если сервер остановили раньше, чем Angular успел запуститься
    private readonly IHostApplicationLifetime _applicationLifetime;

    // Дочерний процесс cmd.exe → npm start → ng serve
    private Process? _process;

    // Токен отмены для фоновой задачи ожидания Angular.
    // Срабатывает при остановке API ИЛИ при явной отмене (StopAsync).
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

    /// <summary>
    /// Вызывается ASP.NET Core при старте приложения.
    /// Запускает npm start в папке Angular-клиента.
    /// </summary>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Вычисляем абсолютный путь к папке Angular-клиента.
        // Структура: src/AddressManager.Api/../AddressManager.Client → src/AddressManager.Client
        var clientPath = Path.GetFullPath(
            Path.Combine(_env.ContentRootPath, "..", "AddressManager.Client"));

        // Если папка клиента не найдена — просто пропускаем запуск (не падаем)
        if (!Directory.Exists(clientPath))
        {
            _logger.LogWarning("Angular client folder not found at {Path}. Skipping.", clientPath);
            return Task.CompletedTask;
        }

        _logger.LogInformation("Starting Angular dev server at {Path}...", clientPath);

        // Настраиваем процесс: cmd.exe запускает скрипт
        // "если нет node_modules — установи зависимости (npm ci), затем запусти сервер (npm start)"
        // npm ci — быстрая чистая установка строго по package-lock.json
        _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName  = "cmd.exe",
                Arguments = "/d /s /c \"if not exist node_modules (npm ci) && npm start\"",
                WorkingDirectory = clientPath,
                UseShellExecute  = false,   // нужно для перенаправления вывода
                CreateNoWindow   = true,    // не показываем отдельное окно cmd
                RedirectStandardOutput = true,
                RedirectStandardError  = true
            }
        };

        try
        {
            _process.Start();

            // Перенаправляем stdout и stderr Angular в логи ASP.NET Core,
            // чтобы видеть прогресс компиляции прямо в консоли API
            _process.OutputDataReceived += (_, e) => LogAngularOutput(e.Data, LogLevel.Information);
            _process.ErrorDataReceived  += (_, e) => LogAngularOutput(e.Data, LogLevel.Warning);
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            // Создаём связанный токен отмены: срабатывает при остановке API или при StopAsync
            _readinessCancellation = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                _applicationLifetime.ApplicationStopping);

            // Запускаем фоновую задачу ожидания Angular (не блокируем StartAsync)
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

        // Возвращаемся немедленно — Angular компилируется в фоне
        return Task.CompletedTask;
    }

    /// <summary>
    /// Вызывается ASP.NET Core при остановке приложения (Ctrl+C, завершение процесса).
    /// Завершает дочерний процесс Angular.
    /// </summary>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Отменяем фоновую задачу ожидания, чтобы она не зависла
        _readinessCancellation?.Cancel();

        if (_process is { HasExited: false })
        {
            _logger.LogInformation("Stopping Angular dev server (PID {Pid})...", _process.Id);
            try
            {
                // entireProcessTree: true — убиваем cmd.exe и все его дочерние процессы
                // (иначе node.exe остался бы висеть и занимал порт 4200)
                _process.Kill(entireProcessTree: true);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not kill Angular process.");
            }
        }
        return Task.CompletedTask;
    }

    /// <summary>Освобождаем неуправляемые ресурсы (Process, CancellationTokenSource).</summary>
    public void Dispose()
    {
        _readinessCancellation?.Dispose();
        _process?.Dispose();
    }

    // Выводим строку лога Angular в логгер ASP.NET Core.
    // Пустые строки пропускаем, чтобы не засорять вывод.
    private void LogAngularOutput(string? line, LogLevel level)
    {
        if (!string.IsNullOrWhiteSpace(line))
        {
            _logger.Log(level, "Angular: {Message}", line);
        }
    }

    /// <summary>
    /// Каждую секунду пингует http://localhost:4200.
    /// Как только Angular вернёт 200 OK — открывает браузер.
    /// Максимальное ожидание: 90 секунд (первый запуск с npm ci может быть долгим).
    /// </summary>
    private async Task WaitForAngularAndOpenBrowserAsync(CancellationToken cancellationToken)
    {
        // Короткий таймаут HttpClient: если Angular не ответил за 1 сек — ещё компилируется
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(1) };

        for (var attempt = 0; attempt < 90 && !cancellationToken.IsCancellationRequested; attempt++)
        {
            try
            {
                using var response = await client.GetAsync(AngularUrl, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Angular is ready at {Url}; opening the browser.", AngularUrl);
                    // UseShellExecute = true — открываем URL в браузере по умолчанию
                    Process.Start(new ProcessStartInfo(AngularUrl) { UseShellExecute = true });
                    return;
                }
            }
            catch (HttpRequestException)
            {
                // Angular ещё не поднялся — ждём следующей попытки
            }
            catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // API остановили раньше, чем Angular успел запуститься — выходим тихо
                return;
            }

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }

        // Прошло 90 секунд, Angular так и не ответил — скорее всего ошибка в npm/ng
        if (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(
                "Angular did not become ready at {Url}. Check the API debug output for npm errors.",
                AngularUrl);
        }
    }
}
