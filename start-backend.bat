@echo off
title Recruitment API Server
cd /d "%~dp0backend\API"

:: Kill any old process on port 5136
for /f "tokens=5" %%a in ('netstat -ano ^| findstr :5136 ^| findstr LISTENING') do (
    taskkill /f /pid %%a >nul 2>&1
)
timeout /t 2 /nobreak >nul

echo ========================================
echo  Recruitment API Server
echo ========================================
echo.
dotnet run
echo.
echo Server stopped.
pause
