@echo off
title Recruitment Frontend
cd /d "%~dp0frontend"

:: Kill any old process on port 3000
for /f "tokens=5" %%a in ('netstat -ano ^| findstr :3000 ^| findstr LISTENING') do (
    taskkill /f /pid %%a >nul 2>&1
)
timeout /t 2 /nobreak >nul

echo ========================================
echo  Recruitment Frontend
echo ========================================
echo.
npm run dev
echo.
pause
