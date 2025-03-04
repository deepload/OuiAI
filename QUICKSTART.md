# OuiAI Quick Start Guide

This guide provides a quick path to get OuiAI up and running on a Windows machine.

## Prerequisites

Ensure you have the following installed:
- Windows 10/11
- .NET 6.0 SDK
- SQL Server (Express or Developer)
- Node.js (LTS version)
- Git
- Visual Studio 2022 (recommended)

## Quick Install (5 Minutes)

### Step 1: Clone the Repository

```powershell
# Open PowerShell as Administrator
mkdir C:\Projects
cd C:\Projects
git clone https://github.com/yourusername/OuiAI.git
cd OuiAI
```

### Step 2: Run the Setup Script

```powershell
# This script will set up everything automatically
.\scripts\setup-dev-environment.ps1
```

The setup script will:
- Create required databases
- Restore NuGet packages
- Install npm dependencies
- Apply database migrations
- Configure environment variables

### Step 3: Start the Backend

```powershell
# Option 1: Using the script
.\scripts\start-backend.ps1

# Option 2: Using Visual Studio
# Open src\Backend\OuiAI.Backend.sln and press F5
```

### Step 4: Start the Web Frontend

```powershell
cd src\Frontend\OuiAI.Web\ouiai-web
npm start
```

Your default browser will open to `http://localhost:3000`

### Step 5: Start the Mobile Frontend (Optional)

```powershell
cd src\Frontend\OuiAI.Mobile\OuiAI
npx expo start
```

Scan the QR code with the Expo Go app on your mobile device.

## Default Login Credentials

- **Admin User**:
  - Email: admin@ouiai.com
  - Password: Admin@123

- **Demo User**:
  - Email: demo@ouiai.com
  - Password: Demo@123

## Quick Tour

### Web Application

1. **Login**: Navigate to http://localhost:3000/login
2. **Social Feed**: Explore the social feed at http://localhost:3000/social
3. **Messages**: Check your messages at http://localhost:3000/messages
4. **Profile**: View your profile at http://localhost:3000/profile

### Mobile Application

1. **Login**: Open the app and tap on "Login"
2. **Home Tab**: Explore the social feed
3. **Messages Tab**: Check your messages
4. **Profile Tab**: View your profile

## Troubleshooting

### Common Issues

1. **Port conflicts**: If ports are in use, modify them in:
   - Backend: `properties/launchSettings.json`
   - Web Frontend: `.env` file

2. **Database connection failures**:
   - Verify SQL Server is running
   - Check connection strings in `appsettings.Development.json`

3. **SignalR connection issues**:
   - Ensure all backend services are running
   - Check CORS configuration

### Quick Fixes

```powershell
# Reset development environment
.\scripts\reset-dev-environment.ps1

# Clear database and apply migrations
.\scripts\reset-database.ps1

# Clear npm cache and reinstall dependencies
.\scripts\reset-frontend.ps1
```

## Next Steps

For more detailed information, refer to:
- [Complete Installation Guide](INSTALLATION.md)
- [Testing Guide](TESTING.md)
- [API Documentation](docs/api/README.md)

## Getting Help

- Join our Discord server: [https://discord.gg/ouiai](https://discord.gg/ouiai)
- Email support: support@ouiai.com
- GitHub issues: [https://github.com/yourusername/OuiAI/issues](https://github.com/yourusername/OuiAI/issues)
