# OuiAI Architecture Documentation

## System Overview

OuiAI is built using a microservices architecture that enables scalability, maintainability, and separation of concerns. This document provides an overview of the system architecture and explains how the various components interact.

## Table of Contents

- [Architecture Overview](#architecture-overview)
- [Microservices](#microservices)
- [Frontend Applications](#frontend-applications)
- [Infrastructure](#infrastructure)
- [Data Flow](#data-flow)
- [Authentication & Authorization](#authentication--authorization)
- [Deployment Architecture](#deployment-architecture)

## Architecture Overview

![OuiAI Architecture Diagram](./images/ouiai-architecture.png)

OuiAI follows a microservices architecture pattern with the following key components:

1. **API Gateway**: Single entry point for all client requests
2. **Microservices**: Specialized services that handle specific business domains
3. **Message Bus**: Handles async communication between services
4. **Databases**: Separate databases for each microservice
5. **Frontend Applications**: Web and mobile client applications
6. **Infrastructure Services**: Supporting services like logging, monitoring, and caching

## Microservices

The backend is composed of several microservices, each responsible for a specific domain:

### Gateway Service

The API Gateway serves as the entry point for all client requests. It:
- Routes requests to appropriate microservices
- Handles authentication token validation
- Provides API documentation via Swagger
- Implements cross-cutting concerns like CORS and request logging

**Technology**: ASP.NET Core 6.0, Ocelot

### Identity Service

Manages user authentication and authorization:
- User registration and login
- JWT token issuance and validation
- User profile management
- Role-based access control

**Technology**: ASP.NET Core Identity, JWT Authentication

### Projects Service

Handles AI project management:
- Project creation, updating, and deletion
- Project categorization and tagging
- Project versioning
- Project permissions

**Technology**: ASP.NET Core, Entity Framework Core, SQL Server

### Social Service

Manages social interactions:
- Following/unfollowing users
- Activity feed generation
- Likes, comments, and sharing
- Content moderation

**Technology**: ASP.NET Core, Entity Framework Core, SQL Server, SignalR

### Notifications Service

Handles real-time notifications:
- Event-based notification generation
- Notification delivery (in-app, email)
- Notification preferences
- Notification history

**Technology**: ASP.NET Core, Azure Service Bus, SignalR, SendGrid

### Search Service

Provides search and recommendation functionality:
- Full-text search across projects and users
- Relevance ranking
- Search filters
- Recommendation algorithms

**Technology**: ASP.NET Core, Azure Cognitive Search, Azure ML

## Frontend Applications

OuiAI provides both web and mobile client applications, built using a shared component architecture:

### Web Application

- Single-page application (SPA)
- Responsive design for desktop and mobile browsers
- Dark mode with mystical theme elements

**Technology**: React, Redux, Axios, Material-UI

### Mobile Application

- Cross-platform mobile application
- Native-like experience on iOS and Android
- Offline capabilities and push notifications

**Technology**: React Native, Expo, Redux, Axios

## Infrastructure

### Database Architecture

Each microservice maintains its own database to ensure loose coupling and independent scalability:

- **Identity DB**: User accounts, profiles, and roles
- **Projects DB**: Project metadata, versions, and categories
- **Social DB**: Social connections, activities, and interactions
- **Notifications DB**: Notification templates and delivery status
- **Search DB**: Search indexes and analytics

**Technology**: SQL Server, Azure SQL Database, Entity Framework Core

### Message Bus

The system uses a message bus for asynchronous communication between microservices:

- **Events**: Services publish domain events when state changes
- **Commands**: Services send commands to request actions from other services
- **Integration**: Enables eventual consistency across service boundaries

**Technology**: Azure Service Bus, MassTransit

### Caching

Multi-level caching strategy:
- **Memory Cache**: In-process caching for frequently accessed data
- **Distributed Cache**: Shared cache for data that needs to be accessed across services
- **CDN**: Content delivery network for static assets

**Technology**: Redis, Azure CDN

## Data Flow

### Request Flow

1. Client sends request to API Gateway
2. Gateway authenticates and routes request to appropriate microservice
3. Microservice processes request and interacts with its database
4. Microservice publishes events to the message bus if state changes
5. Other services subscribe to relevant events and update their state
6. Response is returned to the client via the Gateway

### Real-time Updates

1. Client establishes WebSocket connection via SignalR
2. When state changes occur, notifications are pushed to connected clients
3. Client updates UI in response to real-time events

## Authentication & Authorization

### Authentication Flow

1. User logs in through the Identity Service
2. Identity Service validates credentials and issues JWT token
3. Client includes JWT token in subsequent API requests
4. Gateway validates token before routing requests to microservices
5. Microservices use token claims for authorization decisions

### Authorization

- **Role-based Access Control**: Permissions based on user roles
- **Resource-based Authorization**: Permissions based on resource ownership
- **Claims-based Authorization**: Permissions based on user attributes

## Deployment Architecture

### Azure Kubernetes Service (AKS)

The production environment runs on AKS with the following configuration:
- Separate pods for each microservice
- Horizontal Pod Autoscaling based on CPU/memory usage
- Ingress controller for routing external traffic
- Persistent volumes for stateful services

### CI/CD Pipeline

Continuous integration and deployment using Azure DevOps:
- Code quality checks and unit tests
- Container building and pushing to Azure Container Registry
- Automated deployment to staging environment
- Manual approval for production deployment
- Blue-green deployment strategy to minimize downtime

### Monitoring and Logging

- **Application Insights**: Performance monitoring and application logging
- **Azure Monitor**: Infrastructure monitoring and alerting
- **Log Analytics**: Centralized logging and analysis
- **Grafana/Prometheus**: Custom metrics and dashboards
