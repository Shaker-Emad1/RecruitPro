# AI Recruitment Automation Platform - Runner Script
param(
    [switch]$NoBuild,
    [switch]$NoSeed
)

$Red = "Red"
$Green = "Green"
$Yellow = "Yellow"
$Cyan = "Cyan"

function Write-Color($color, $msg) { Write-Host $msg -ForegroundColor $color }

$rootDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$backendDir = Join-Path $rootDir "backend\API"
$frontendDir = Join-Path $rootDir "frontend"
$apiUrl = "http://localhost:5136"
$frontendUrl = "http://localhost:3000"

Write-Color $Cyan "========================================"
Write-Color $Cyan "  AI Recruitment Automation Platform"
Write-Color $Cyan "========================================"
Write-Host ""

# Check prerequisites
Write-Color $Yellow "[1/4] Checking prerequisites..."

$dotnetVersion = dotnet --version 2>$null
if (-not $dotnetVersion) {
    Write-Color $Red "ERROR: .NET SDK not found. Install from https://dotnet.microsoft.com/download"
    exit 1
}
Write-Color $Green "  .NET SDK $dotnetVersion"

$nodeVersion = node --version 2>$null
if (-not $nodeVersion) {
    Write-Color $Red "ERROR: Node.js not found. Install from https://nodejs.org"
    exit 1
}
Write-Color $Green "  Node.js $nodeVersion"

# Kill any existing processes on our ports
Get-NetTCPConnection -LocalPort 5136 -ErrorAction SilentlyContinue | ForEach-Object {
    $proc = Get-Process -Id $_.OwningProcess -ErrorAction SilentlyContinue
    if ($proc -and $proc.ProcessName -eq "dotnet") {
        Write-Color $Yellow "  Killing old dotnet process on port 5136 (PID $($_.OwningProcess))"
        Stop-Process -Id $_.OwningProcess -Force -ErrorAction SilentlyContinue
    }
}
Get-NetTCPConnection -LocalPort 3000 -ErrorAction SilentlyContinue | ForEach-Object {
    $proc = Get-Process -Id $_.OwningProcess -ErrorAction SilentlyContinue
    if ($proc -and $proc.ProcessName -eq "node") {
        Write-Color $Yellow "  Killing old node process on port 3000 (PID $($_.OwningProcess))"
        Stop-Process -Id $_.OwningProcess -Force -ErrorAction SilentlyContinue
    }
}
Start-Sleep 2

# Build backend if needed
if (-not $NoBuild) {
    Write-Color $Yellow "[2/4] Building Backend..."
    $buildResult = & dotnet build $backendDir --nologo 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Color $Red "ERROR: Backend build failed"
        Write-Host $buildResult -ForegroundColor $Red
        exit 1
    }
    Write-Color $Green "  Backend build successful"
}

# Start backend
Write-Color $Yellow "[3/4] Starting Backend API..."
$backendJob = Start-Job -Name "BackendAPI" -ScriptBlock {
    param($dir) Set-Location $dir; dotnet run --no-build
} -ArgumentList $backendDir

# Wait for backend to be ready
$maxWait = 60
$waited = 0
$ready = $false
while ($waited -lt $maxWait) {
    Start-Sleep 2
    $waited += 2
    try {
        $r = Invoke-WebRequest -Uri "$apiUrl/swagger/v1/swagger.json" -UseBasicParsing -TimeoutSec 3
        if ($r.StatusCode -eq 200) { $ready = $true; break }
    } catch {}
    if ($waited -eq 10) { Write-Color $Yellow "  Waiting for backend (seeding data may take a minute)..." }
}

if (-not $ready) {
    Write-Color $Red "ERROR: Backend failed to start within $maxWait seconds"
    Write-Color $Yellow "Check if another process is using port 5136"
    exit 1
}
Write-Color $Green "  Backend API ready at $apiUrl"

# Build and start frontend
Write-Color $Yellow "[4/4] Starting Frontend..."
$frontendJob = Start-Job -Name "Frontend" -ScriptBlock {
    param($dir) Set-Location $dir; npm run dev
} -ArgumentList $frontendDir

Start-Sleep 8

# Check if frontend started
try {
    $r = Invoke-WebRequest -Uri $frontendUrl -UseBasicParsing -TimeoutSec 5
    if ($r.StatusCode -eq 200) { Write-Color $Green "  Frontend ready at $frontendUrl" }
    else { Write-Color $Yellow "  Frontend starting..." }
} catch {
    Write-Color $Yellow "  Frontend still starting (usually takes 10-15s)..."
}

Write-Host ""
Write-Color $Green "========================================"
Write-Color $Green "  SYSTEM READY!"
Write-Color $Green "========================================"
Write-Host ""
Write-Host "  Backend API:    " -NoNewline; Write-Color $Cyan $apiUrl
Write-Host "  Swagger UI:     " -NoNewline; Write-Color $Cyan "$apiUrl/swagger"
Write-Host "  Frontend:       " -NoNewline; Write-Color $Cyan $frontendUrl
Write-Host ""
Write-Host "  Login Credentials:"
Write-Host "    Admin:     admin@recruit.com / Admin123!"
Write-Host "    HR Manager: sarah@recruit.com / HrManager123!"
Write-Host "    Recruiter:  michael@recruit.com / Recruiter123!"
Write-Host ""
Write-Host "  Press ANY KEY to stop all servers and exit."

# Open browsers
Start-Process $apiUrl
Start-Process $frontendUrl

# Wait for key press to shut down
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

Write-Host ""
Write-Color $Yellow "Shutting down..."
Stop-Job $backendJob -ErrorAction SilentlyContinue
Stop-Job $frontendJob -ErrorAction SilentlyContinue
Remove-Job $backendJob -Force -ErrorAction SilentlyContinue
Remove-Job $frontendJob -Force -ErrorAction SilentlyContinue

# Make sure ports are freed
Get-NetTCPConnection -LocalPort 5136 -ErrorAction SilentlyContinue | ForEach-Object {
    Stop-Process -Id $_.OwningProcess -Force -ErrorAction SilentlyContinue
}
Get-NetTCPConnection -LocalPort 3000 -ErrorAction SilentlyContinue | ForEach-Object {
    Stop-Process -Id $_.OwningProcess -Force -ErrorAction SilentlyContinue
}

Write-Color $Green "Done."
