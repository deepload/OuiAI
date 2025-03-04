# OuiAI Backend Startup Script
# This script starts all backend microservices

Write-Host "Starting OuiAI backend services..." -ForegroundColor Cyan

# Define service information
$services = @(
    @{
        Name = "Gateway"
        Path = "src\Backend\OuiAI.Microservices.Gateway\OuiAI.Microservices.Gateway"
        Port = 5001
    },
    @{
        Name = "Identity"
        Path = "src\Backend\OuiAI.Microservices.Identity\OuiAI.Microservices.Identity"
        Port = 5002
    },
    @{
        Name = "Projects"
        Path = "src\Backend\OuiAI.Microservices.Projects\OuiAI.Microservices.Projects"
        Port = 5003
    },
    @{
        Name = "Social"
        Path = "src\Backend\OuiAI.Microservices.Social\OuiAI.Microservices.Social"
        Port = 5004
    },
    @{
        Name = "Notifications"
        Path = "src\Backend\OuiAI.Microservices.Notifications\OuiAI.Microservices.Notifications"
        Port = 5005
    },
    @{
        Name = "Search"
        Path = "src\Backend\OuiAI.Microservices.Search\OuiAI.Microservices.Search"
        Port = 5006
    }
)

# Check if services are already running
foreach ($service in $services) {
    $netstatOutput = netstat -ano | findstr $service.Port
    if ($netstatOutput) {
        Write-Host "WARNING: Port $($service.Port) is already in use. $($service.Name) may already be running." -ForegroundColor Yellow
    }
}

# Start each service in a new PowerShell window
foreach ($service in $services) {
    Write-Host "Starting $($service.Name) service on port $($service.Port)..." -ForegroundColor Yellow
    
    # Create a script block for the service
    $scriptBlock = {
        param($servicePath, $serviceName)
        
        Set-Location -Path $servicePath
        Write-Host "Starting $serviceName service..."
        dotnet run
    }
    
    # Start a new PowerShell window with the script block
    Start-Process powershell -ArgumentList "-NoExit", "-Command", 
        "& {Set-Location '$($service.Path)'; Write-Host 'Starting $($service.Name) service...' -ForegroundColor Cyan; dotnet run}"
}

Write-Host "All services have been started!" -ForegroundColor Green
Write-Host "NOTE: Each service is running in its own PowerShell window." -ForegroundColor Yellow
Write-Host "To stop a service, close its corresponding PowerShell window." -ForegroundColor Yellow

# Tell the user how to access the services
Write-Host "`nAccess points:" -ForegroundColor Cyan
foreach ($service in $services) {
    Write-Host "$($service.Name): https://localhost:$($service.Port)" -ForegroundColor Green
}

Write-Host "`nSwagger API documentation:" -ForegroundColor Cyan
foreach ($service in $services) {
    Write-Host "$($service.Name): https://localhost:$($service.Port)/swagger" -ForegroundColor Green
}
