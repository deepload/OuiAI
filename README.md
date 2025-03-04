# OuiAI Platform

## Overview
OuiAI is a professional platform dedicated to hosting, discovering, and sharing AI projects. It serves as an interactive community where creators can publish their AI works across various categories, exchange ideas, and collaborate with peers.

The platform features a mystical, Ouija-inspired design theme while maintaining professional functionality through a modern, dark-mode interface with subtle interactive elements.

## Architecture

### Backend
- **Technology**: .NET Core 6.0 with microservices architecture
- **Cloud Services**: Azure (AKS, Service Bus, SQL Database, Blob Storage, Redis Cache)
- **Authentication**: Azure Active Directory
- **Security**: Azure Key Vault, SSL encryption
- **API Documentation**: Swagger UI

### Frontend
- **Technology**: React Native for cross-platform compatibility (iOS, Android, Web)
- **Design**: Dark mode, responsive, with subtle mystical themes
- **State Management**: Redux
- **API Communication**: Axios

## Core Features
- User authentication and profile management
- Project submission and categorization
- Interactive social elements (likes, comments, sharing)
- Real-time notifications via Azure Service Bus
- Advanced search and recommendation system
- Direct messaging between users

## Project Structure

```
OuiAI/
├── src/
│   ├── Backend/
│   │   ├── OuiAI.Microservices.Gateway/         # API Gateway
│   │   ├── OuiAI.Microservices.Identity/        # User management & authentication
│   │   ├── OuiAI.Microservices.Projects/        # Project management
│   │   ├── OuiAI.Microservices.Social/          # Social interactions
│   │   ├── OuiAI.Microservices.Notifications/   # Real-time notifications
│   │   ├── OuiAI.Microservices.Search/          # Search and recommendations
│   │   └── OuiAI.Common/                        # Shared libraries and models
│   │
│   └── Frontend/
│       ├── OuiAI.Mobile/                      # React Native mobile app (iOS/Android)
│       └── OuiAI.Web/                         # React Native Web
│
├── infrastructure/
│   ├── kubernetes/                           # Kubernetes configuration
│   ├── azure-pipelines/                      # CI/CD configuration
│   └── terraform/                            # Infrastructure as Code
│
├── docs/
│   ├── architecture/                         # Architecture documentation
│   ├── api/                                  # API documentation
│   └── development-guide/                    # Development guidelines
│
└── tests/
    ├── Backend.Tests/                        # Backend unit tests
    └── Frontend.Tests/                       # Frontend unit tests
```

## Getting Started

### Prerequisites
- .NET Core 6.0 SDK
- Node.js 16+ and npm/yarn
- Docker and Docker Compose
- Azure CLI
- Kubernetes CLI (kubectl)

### Local Development Setup
1. Clone the repository
2. Set up local development environment (see docs/development-guide)
3. Run Docker Compose to start required services
4. Start backend services
5. Start frontend application

## Deployment
The application is configured for deployment on Azure Kubernetes Service (AKS) using CI/CD pipelines through Azure DevOps.

## Contributing
Please read our contribution guidelines in the docs/development-guide directory.

## License
[Specify your license]
