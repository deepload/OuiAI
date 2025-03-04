# OuiAI Development Environment Setup Script
# This script sets up everything needed for local development

Write-Host "Setting up OuiAI development environment..." -ForegroundColor Cyan

# Check prerequisites
Write-Host "Checking prerequisites..." -ForegroundColor Yellow

# Check .NET Core SDK
try {
    $dotnetVersion = dotnet --version
    Write-Host ".NET SDK version: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "ERROR: .NET SDK not found. Please install .NET 6.0 SDK or later." -ForegroundColor Red
    exit 1
}

# Check Node.js
try {
    $nodeVersion = node --version
    Write-Host "Node.js version: $nodeVersion" -ForegroundColor Green
} catch {
    Write-Host "ERROR: Node.js not found. Please install Node.js 16.x or later." -ForegroundColor Red
    exit 1
}

# Check SQL Server
try {
    # This is a simplified check, might need adjustment based on your SQL Server setup
    $sqlServerInstance = "localhost\SQLEXPRESS"
    $connectionString = "Server=$sqlServerInstance;Database=master;Trusted_Connection=True;"
    
    # Skip actual connection test for now - would require SqlClient package
    Write-Host "SQL Server check: Assuming SQL Server is installed" -ForegroundColor Yellow
    Write-Host "NOTE: If you encounter SQL connection issues, please verify your SQL Server installation" -ForegroundColor Yellow
} catch {
    Write-Host "WARNING: Could not verify SQL Server installation." -ForegroundColor Yellow
}

# Create project directories if they don't exist
Write-Host "Creating directory structure..." -ForegroundColor Yellow

$directories = @(
    "logs",
    "temp"
)

foreach ($dir in $directories) {
    if (-not (Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir | Out-Null
        Write-Host "Created directory: $dir" -ForegroundColor Green
    }
}

# Create SQL databases
Write-Host "Setting up databases..." -ForegroundColor Yellow

$databaseScript = @"
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OuiAI_Identity')
BEGIN
    CREATE DATABASE OuiAI_Identity;
END

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OuiAI_Projects')
BEGIN
    CREATE DATABASE OuiAI_Projects;
END

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OuiAI_Social')
BEGIN
    CREATE DATABASE OuiAI_Social;
END

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OuiAI_Notifications')
BEGIN
    CREATE DATABASE OuiAI_Notifications;
END

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OuiAI_Search')
BEGIN
    CREATE DATABASE OuiAI_Search;
END
"@

$sqlScriptPath = ".\temp\create-databases.sql"
$databaseScript | Out-File -FilePath $sqlScriptPath -Encoding utf8

Write-Host "SQL script created at $sqlScriptPath" -ForegroundColor Green
Write-Host "Please run this script manually in SQL Server Management Studio or using sqlcmd" -ForegroundColor Yellow

# Restore NuGet packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
Set-Location -Path "src\Backend"
dotnet restore
Set-Location -Path "..\..\"
Write-Host "NuGet packages restored" -ForegroundColor Green

# Install frontend dependencies
Write-Host "Installing Web Frontend dependencies..." -ForegroundColor Yellow
Set-Location -Path "src\Frontend\OuiAI.Web\ouiai-web"
npm install
Set-Location -Path "..\..\..\.."
Write-Host "Web Frontend dependencies installed" -ForegroundColor Green

Write-Host "Installing Mobile Frontend dependencies..." -ForegroundColor Yellow
Set-Location -Path "src\Frontend\OuiAI.Mobile\OuiAI"
npm install
Set-Location -Path "..\..\..\.."
Write-Host "Mobile Frontend dependencies installed" -ForegroundColor Green

# Create environment variable
Write-Host "Setting environment variables..." -ForegroundColor Yellow
[System.Environment]::SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development", [System.EnvironmentVariableTarget]::User)
Write-Host "Environment variables set" -ForegroundColor Green

# Create local settings files if they don't exist
Write-Host "Creating local settings files..." -ForegroundColor Yellow

$microservices = @(
    "Gateway",
    "Identity",
    "Projects",
    "Social",
    "Notifications",
    "Search"
)

foreach ($service in $microservices) {
    $settingsPath = "src\Backend\OuiAI.Microservices.$service\OuiAI.Microservices.$service\appsettings.Development.json"
    
    if (-not (Test-Path $settingsPath)) {
        $appsettingsContent = @"
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "${service}Connection": "Server=localhost\\SQLEXPRESS;Database=OuiAI_$service;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Jwt": {
    "Key": "YourDevSecretKeyHere-PleaseChangeInProduction",
    "Issuer": "OuiAI.Identity",
    "Audience": "OuiAI.Services",
    "ExpiryMinutes": 60
  },
  "AllowedHosts": "*",
  "CorsOrigins": [
    "http://localhost:3000",
    "http://localhost:19006"
  ]
}
"@
        $appsettingsContent | Out-File -FilePath $settingsPath -Encoding utf8
        Write-Host "Created settings file: $settingsPath" -ForegroundColor Green
    }
}

# Create frontend environment files
$webEnvPath = "src\Frontend\OuiAI.Web\ouiai-web\.env"
if (-not (Test-Path $webEnvPath)) {
    $webEnvContent = @"
REACT_APP_API_URL=https://localhost:5001
REACT_APP_AUTH_URL=https://localhost:5002
REACT_APP_SIGNALR_URL=https://localhost:5001/hubs
"@
    $webEnvContent | Out-File -FilePath $webEnvPath -Encoding utf8
    Write-Host "Created Web Frontend environment file" -ForegroundColor Green
}

$mobileEnvPath = "src\Frontend\OuiAI.Mobile\OuiAI\.env"
if (-not (Test-Path $mobileEnvPath)) {
    $mobileEnvContent = @"
API_URL=https://localhost:5001
AUTH_URL=https://localhost:5002
SIGNALR_URL=https://localhost:5001/hubs
"@
    $mobileEnvContent | Out-File -FilePath $mobileEnvPath -Encoding utf8
    Write-Host "Created Mobile Frontend environment file" -ForegroundColor Green
}

Write-Host "Setup completed successfully!" -ForegroundColor Cyan
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Run the SQL script in SQL Server Management Studio: $sqlScriptPath" -ForegroundColor Yellow
Write-Host "2. Run migrations with: dotnet ef database update (in each microservice directory)" -ForegroundColor Yellow
Write-Host "3. Start the backend with: .\scripts\start-backend.ps1" -ForegroundColor Yellow
Write-Host "4. Start the web frontend with: cd src\Frontend\OuiAI.Web\ouiai-web && npm start" -ForegroundColor Yellow
