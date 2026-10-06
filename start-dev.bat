@echo off
echo ========================================
echo   AddressManager — Dev Launcher
echo ========================================
echo.
echo [1/2] Starting .NET API on http://localhost:5017
echo       (Angular will start automatically via API)
echo.
start "AddressManager API" cmd /k "cd /d %~dp0src\AddressManager.Api && dotnet run"
echo.
echo Done! Open http://localhost:4200 in a few seconds.
echo (Angular dev server starts with the API automatically)
echo.
pause
