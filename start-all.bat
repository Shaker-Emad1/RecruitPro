@echo off
title Recruitment Platform
cd /d "%~dp0"

echo ========================================
echo  AI Recruitment Automation Platform
echo ========================================
echo.
echo  Starting Backend API...
start "Recruitment API" "%~dp0start-backend.bat"

echo  Waiting for backend...
timeout /t 8 /nobreak >nul

echo  Starting Frontend...
start "Recruitment Frontend" "%~dp0start-frontend.bat"

echo.
echo ========================================
echo  Opening browsers...
echo ========================================
echo.
echo  Backend API:    http://localhost:5136
echo  Swagger UI:     http://localhost:5136/swagger
echo  Frontend:       http://localhost:3000
echo.
echo  Wait 10-15 seconds for frontend to be ready.
echo.
timeout /t 5 /nobreak >nul

:: Open in default browser
start http://localhost:5136/swagger
timeout /t 2 /nobreak >nul
start http://localhost:3000

echo.
echo  To stop: Close the two server windows.
echo.
pause
