# OuiAI Test Execution Script
# This script runs all tests for the OuiAI platform

param (
    [switch]$BackendOnly,
    [switch]$FrontendOnly,
    [switch]$Coverage,
    [string]$TestCategory,
    [switch]$CI
)

Write-Host "OuiAI Test Runner" -ForegroundColor Cyan
Write-Host "==================" -ForegroundColor Cyan

# Define paths
$backendTestsPath = "tests\Backend.Tests"
$webFrontendPath = "src\Frontend\OuiAI.Web\ouiai-web"
$mobileFrontendPath = "src\Frontend\OuiAI.Mobile\OuiAI"
$testReportPath = "test-reports"

# Create test report directory if it doesn't exist
if (-not (Test-Path $testReportPath)) {
    New-Item -ItemType Directory -Path $testReportPath | Out-Null
    Write-Host "Created directory: $testReportPath" -ForegroundColor Green
}

# Function to run .NET tests
function Run-Backend-Tests {
    param (
        [bool]$WithCoverage = $false,
        [string]$Category = ""
    )
    
    Write-Host "`nRunning Backend Tests..." -ForegroundColor Yellow
    
    Push-Location $backendTestsPath
    
    $testCommand = "dotnet test"
    
    if ($Category) {
        $testCommand += " --filter `"Category=$Category`""
    }
    
    if ($WithCoverage) {
        Write-Host "Generating code coverage..." -ForegroundColor Yellow
        
        # Check if coverlet is installed, if not install it
        $coverletExists = dotnet tool list --global | Select-String -Pattern "coverlet.console"
        if (-not $coverletExists) {
            Write-Host "Installing coverlet..." -ForegroundColor Yellow
            dotnet tool install --global coverlet.console
        }
        
        # Check if report generator is installed, if not install it
        $reportgenExists = dotnet tool list --global | Select-String -Pattern "dotnet-reportgenerator"
        if (-not $reportgenExists) {
            Write-Host "Installing ReportGenerator..." -ForegroundColor Yellow
            dotnet tool install --global dotnet-reportgenerator-globaltool
        }
        
        # Run tests with coverage
        $testCommand += " /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=`"../../$testReportPath/backend-coverage.cobertura.xml`""
        
        Invoke-Expression $testCommand
        
        # Generate HTML report
        reportgenerator -reports:"../../$testReportPath/backend-coverage.cobertura.xml" -targetdir:"../../$testReportPath/backend-coverage-report" -reporttypes:Html
        
        Write-Host "Coverage report generated: $testReportPath/backend-coverage-report/index.html" -ForegroundColor Green
    } else {
        Invoke-Expression $testCommand
    }
    
    Pop-Location
}

# Function to run Web Frontend tests
function Run-Web-Frontend-Tests {
    param (
        [bool]$WithCoverage = $false,
        [bool]$CIMode = $false
    )
    
    Write-Host "`nRunning Web Frontend Tests..." -ForegroundColor Yellow
    
    Push-Location $webFrontendPath
    
    $npmCommand = "npm test"
    
    if ($CIMode) {
        $npmCommand += " -- --ci --watchAll=false"
    }
    
    if ($WithCoverage) {
        $npmCommand += " -- --coverage --coverageDirectory=`"../../$testReportPath/web-coverage`""
        
        Invoke-Expression $npmCommand
        
        Write-Host "Coverage report generated: $testReportPath/web-coverage/lcov-report/index.html" -ForegroundColor Green
    } else {
        if (-not $CIMode) {
            Invoke-Expression $npmCommand
        } else {
            Invoke-Expression $npmCommand
        }
    }
    
    Pop-Location
}

# Function to run Mobile Frontend tests
function Run-Mobile-Frontend-Tests {
    param (
        [bool]$WithCoverage = $false,
        [bool]$CIMode = $false
    )
    
    Write-Host "`nRunning Mobile Frontend Tests..." -ForegroundColor Yellow
    
    Push-Location $mobileFrontendPath
    
    $npmCommand = "npm test"
    
    if ($CIMode) {
        $npmCommand += " -- --ci --watchAll=false"
    }
    
    if ($WithCoverage) {
        $npmCommand += " -- --coverage --coverageDirectory=`"../../../$testReportPath/mobile-coverage`""
        
        Invoke-Expression $npmCommand
        
        Write-Host "Coverage report generated: $testReportPath/mobile-coverage/lcov-report/index.html" -ForegroundColor Green
    } else {
        if (-not $CIMode) {
            Invoke-Expression $npmCommand
        } else {
            Invoke-Expression $npmCommand
        }
    }
    
    Pop-Location
}

# Run the tests based on parameters
if ($BackendOnly -or (-not $FrontendOnly)) {
    Run-Backend-Tests -WithCoverage $Coverage -Category $TestCategory
}

if ($FrontendOnly -or (-not $BackendOnly)) {
    Run-Web-Frontend-Tests -WithCoverage $Coverage -CIMode $CI
    Run-Mobile-Frontend-Tests -WithCoverage $Coverage -CIMode $CI
}

Write-Host "`nAll tests completed!" -ForegroundColor Green

# If we generated coverage reports, give a summary
if ($Coverage) {
    Write-Host "`nTest Coverage Reports:" -ForegroundColor Cyan
    
    if ($BackendOnly -or (-not $FrontendOnly)) {
        Write-Host "Backend: $testReportPath/backend-coverage-report/index.html" -ForegroundColor Green
    }
    
    if ($FrontendOnly -or (-not $BackendOnly)) {
        Write-Host "Web Frontend: $testReportPath/web-coverage/lcov-report/index.html" -ForegroundColor Green
        Write-Host "Mobile Frontend: $testReportPath/mobile-coverage/lcov-report/index.html" -ForegroundColor Green
    }
}

# Display help information
Write-Host "`nUsage Examples:" -ForegroundColor Cyan
Write-Host "  .\run-tests.ps1                     # Run all tests" -ForegroundColor Yellow
Write-Host "  .\run-tests.ps1 -BackendOnly        # Run only backend tests" -ForegroundColor Yellow
Write-Host "  .\run-tests.ps1 -FrontendOnly       # Run only frontend tests" -ForegroundColor Yellow
Write-Host "  .\run-tests.ps1 -Coverage           # Run all tests with coverage" -ForegroundColor Yellow
Write-Host "  .\run-tests.ps1 -TestCategory auth  # Run only tests in 'auth' category" -ForegroundColor Yellow
Write-Host "  .\run-tests.ps1 -CI                 # Run in CI mode (non-interactive)" -ForegroundColor Yellow
