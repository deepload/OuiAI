# OuiAI Installation and Usage Guide

This comprehensive guide will help you set up, run, and test the OuiAI platform on a fresh Windows machine.

## Table of Contents
- [Prerequisites](#prerequisites)
- [Installation Steps](#installation-steps)
  - [Setting up the Development Environment](#setting-up-the-development-environment)
  - [Installing the Backend](#installing-the-backend)
  - [Installing the Web Frontend](#installing-the-web-frontend)
  - [Installing the Mobile Frontend](#installing-the-mobile-frontend)
- [Running the Application](#running-the-application)
  - [Running the Backend Microservices](#running-the-backend-microservices)
  - [Running the Web Frontend](#running-the-web-frontend)
  - [Running the Mobile Frontend](#running-the-mobile-frontend)
- [Testing the Application](#testing-the-application)
  - [Backend Tests](#backend-tests)
  - [Frontend Tests](#frontend-tests)
  - [End-to-End Tests](#end-to-end-tests)
- [Troubleshooting](#troubleshooting)
- [Deployment](#deployment)

## Prerequisites

Before you start, ensure your Windows machine has the following software installed:

1. **Windows 10/11** (64-bit) with latest updates
2. **.NET 6.0 SDK** or newer
   - Download from: https://dotnet.microsoft.com/en-us/download/dotnet/6.0
3. **SQL Server** (Developer or Express edition)
   - Download from: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
4. **SQL Server Management Studio (SSMS)**
   - Download from: https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms
5. **Visual Studio 2022** (Community edition or higher)
   - Download from: https://visualstudio.microsoft.com/vs/
   - Required workloads:
     - ASP.NET and Web Development
     - .NET Desktop Development
     - Azure Development
6. **Node.js** (LTS version, 16.x or newer)
   - Download from: https://nodejs.org/
7. **Git**
   - Download from: https://git-scm.com/downloads
8. **Docker Desktop** (for containerized development and testing)
   - Download from: https://www.docker.com/products/docker-desktop
9. **(Optional) Azure CLI** (for Azure deployments)
   - Download from: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows

## Installation Steps

### Setting up the Development Environment

1. **Clone the Repository**
   ```powershell
   # Create a directory for your projects (if not already existing)
   mkdir C:\Projects
   cd C:\Projects
   
   # Clone the repository
   git clone https://github.com/yourusername/OuiAI.git
   cd OuiAI
   ```

2. **Create Local Configuration Files**
   ```powershell
   # Create local environment files for development
   Copy-Item "infrastructure\local\appsettings.Development.json.template" -Destination "src\Backend\OuiAI.Microservices.Gateway\OuiAI.Microservices.Gateway\appsettings.Development.json"
   
   # Repeat for all microservices (adjust paths as needed)
   ```

3. **Set up Environment Variables**
   - Press Win + R, type `sysdm.cpl`, and press Enter
   - Go to the "Advanced" tab and click "Environment Variables"
   - Create a new system variable:
     - Variable name: `ASPNETCORE_ENVIRONMENT`
     - Variable value: `Development`

### Installing the Backend

1. **Set up SQL Server Databases**
   - Open SQL Server Management Studio and connect to your local SQL Server
   - Execute the following SQL script to create the necessary databases:
   ```sql
   CREATE DATABASE OuiAI_Identity;
   CREATE DATABASE OuiAI_Projects;
   CREATE DATABASE OuiAI_Social;
   CREATE DATABASE OuiAI_Notifications;
   CREATE DATABASE OuiAI_Search;
   GO
   ```

2. **Update Connection Strings**
   - Open each microservice's `appsettings.Development.json` file
   - Update the connection string to match your SQL Server configuration:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=OuiAI_[MicroserviceName];Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```
   - Replace `[MicroserviceName]` with the respective microservice name (Identity, Projects, etc.)
   - If you're using a different SQL Server instance, adjust the server part accordingly

3. **Restore NuGet Packages**
   ```powershell
   cd src\Backend
   dotnet restore OuiAI.Backend.sln
   ```

4. **Apply Entity Framework Migrations**
   ```powershell
   # Navigate to each microservice directory and apply migrations
   cd OuiAI.Microservices.Identity\OuiAI.Microservices.Identity
   dotnet ef database update
   
   # Repeat for all microservices with database contexts
   ```

### Installing the Web Frontend

1. **Install Node.js Dependencies**
   ```powershell
   cd src\Frontend\OuiAI.Web\ouiai-web
   npm install
   # Or if you prefer yarn
   # yarn install
   ```

2. **Configure Environment Variables**
   - Create a `.env` file in the `src\Frontend\OuiAI.Web\ouiai-web` directory:
   ```
   REACT_APP_API_URL=https://localhost:5001
   REACT_APP_AUTH_URL=https://localhost:5002
   REACT_APP_SIGNALR_URL=https://localhost:5001/hubs
   ```

### Installing the Mobile Frontend

1. **Install Node.js Dependencies**
   ```powershell
   cd src\Frontend\OuiAI.Mobile\OuiAI
   npm install
   # Or if you prefer yarn
   # yarn install
   ```

2. **Install Expo CLI Globally**
   ```powershell
   npm install -g expo-cli
   ```

3. **Configure Environment Variables**
   - Create a `.env` file in the `src\Frontend\OuiAI.Mobile\OuiAI` directory with the same content as the web frontend

## Running the Application

### Running the Backend Microservices

#### Option 1: Using Visual Studio

1. Open `src\Backend\OuiAI.Backend.sln` in Visual Studio
2. Right-click on the solution in Solution Explorer and select "Properties"
3. Under "Startup Project," select "Multiple startup projects"
4. Set all microservice projects to "Start"
5. Click "Apply" and "OK"
6. Press F5 or click the Start button to run all microservices

#### Option 2: Using Command Line

1. Open multiple PowerShell windows (one for each microservice)
2. In each window, navigate to the respective microservice directory and run:
   ```powershell
   cd src\Backend\OuiAI.Microservices.[ServiceName]\OuiAI.Microservices.[ServiceName]
   dotnet run
   ```
3. Replace `[ServiceName]` with the respective microservice name (Gateway, Identity, etc.)

#### Option 3: Using Docker Compose

1. Navigate to the root directory
   ```powershell
   cd C:\Projects\OuiAI
   ```
2. Run Docker Compose
   ```powershell
   docker-compose -f infrastructure\docker\docker-compose.yml up -d
   ```

### Running the Web Frontend

1. Navigate to the web frontend directory
   ```powershell
   cd src\Frontend\OuiAI.Web\ouiai-web
   ```

2. Start the development server
   ```powershell
   npm start
   # Or if you prefer yarn
   # yarn start
   ```

3. The application will open in your default browser at `http://localhost:3000`

### Running the Mobile Frontend

1. Navigate to the mobile frontend directory
   ```powershell
   cd src\Frontend\OuiAI.Mobile\OuiAI
   ```

2. Start the Expo development server
   ```powershell
   npx expo start
   ```

3. To run on specific platforms:
   - Press `w` to run in a web browser
   - Press `a` to run on an Android emulator (requires Android Studio)
   - Press `i` to run on an iOS simulator (requires macOS with Xcode)
   - Scan the QR code with the Expo Go app on your physical device

## Testing the Application

### Backend Tests

1. **Running Unit Tests**
   ```powershell
   cd tests\Backend.Tests
   dotnet test
   ```

2. **Running Tests for a Specific Microservice**
   ```powershell
   cd tests\Backend.Tests\OuiAI.Microservices.[ServiceName].Tests
   dotnet test
   ```

### Frontend Tests

1. **Running Web Frontend Tests**
   ```powershell
   cd src\Frontend\OuiAI.Web\ouiai-web
   npm test
   # Or if you prefer yarn
   # yarn test
   ```

2. **Running Mobile Frontend Tests**
   ```powershell
   cd src\Frontend\OuiAI.Mobile\OuiAI
   npm test
   # Or if you prefer yarn
   # yarn test
   ```

### End-to-End Tests

1. **Running Cypress Tests for Web Frontend**
   ```powershell
   cd src\Frontend\OuiAI.Web\ouiai-web
   npm run cypress:open
   # Or if you prefer yarn
   # yarn cypress:open
   ```

## Troubleshooting

### Common Issues and Solutions

1. **Backend Services Won't Start**
   - Check if the required ports are already in use
   - Verify SQL Server is running and accessible
   - Check connection strings in `appsettings.Development.json`
   - Ensure all required NuGet packages are restored

2. **Database Migration Errors**
   - Run `dotnet ef migrations add InitialCreate` if no migrations exist
   - Ensure the database exists and is accessible
   - Check user permissions for the SQL Server account

3. **Frontend Build Errors**
   - Delete the `node_modules` folder and run `npm install` again
   - Clear npm cache with `npm cache clean --force`
   - Update Node.js to the latest LTS version

4. **SignalR Connection Issues**
   - Ensure the SignalR hub URL is correct in the frontend configuration
   - Check if CORS is properly configured in the backend
   - Verify network connectivity between frontend and backend

5. **Docker Issues**
   - Restart Docker Desktop
   - Run `docker system prune` to clean up unused resources
   - Check Docker logs for specific error messages

### Getting Help

If you encounter issues not covered in this guide, please:
1. Check the project's GitHub issues
2. Search the documentation
3. Create a new issue with detailed information about your problem

## Deployment

### Azure Deployment

For production deployment on Azure, follow these steps:

1. **Prepare Azure Resources**
   - Set up Azure App Services or AKS cluster
   - Create SQL databases
   - Configure Azure Service Bus
   - Set up Azure Storage accounts

2. **Configure CI/CD Pipeline**
   - Use Azure DevOps or GitHub Actions
   - Set up the build and release pipelines based on templates in `infrastructure\azure-pipelines`

3. **Deploy Backend Services**
   - Use infrastructure as code (Terraform scripts in `infrastructure\terraform`)
   - Configure environment variables and connection strings in Azure

4. **Deploy Frontend Applications**
   - Build production versions:
     ```powershell
     cd src\Frontend\OuiAI.Web\ouiai-web
     npm run build
     ```
   - Deploy built files to a web hosting service or CDN

### On-Premises Deployment

For on-premises deployment:

1. **Set up a Windows Server**
   - Install IIS and .NET 6.0 Runtime
   - Configure SQL Server

2. **Deploy Backend Services**
   - Publish each microservice using Visual Studio or `dotnet publish`
   - Configure IIS websites and application pools

3. **Deploy Frontend Applications**
   - Build production versions
   - Host static files on IIS or another web server
