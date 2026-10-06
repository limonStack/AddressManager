@echo off
echo ========================================
echo   AddressManager — Dev Launcher
echo ========================================
echo.
echo Script location: %~dp0
echo API path: %~dp0src\AddressManager.Api
echo.
if not exist "%~dp0src\AddressManager.Api" (
    echo ERROR: Path not found: %~dp0src\AddressManager.Api
    pause
    exit /b 1
)
echo [1/2] Starting .NET API on http://localhost:5017
echo       (Angular will start automatically via API)
echo.
start "AddressManager API" cmd /k "cd /d "%~dp0src\AddressManager.Api" && dotnet run"
echo.
echo Done! Open http://localhost:4200 in a few seconds.
echo (Angular dev server starts with the API automatically)
echo.
pause
