# OuiAI Social Microservice

## Overview
The Social microservice implements user social interactions for the OuiAI platform, including:
- User following mechanism
- Real-time notifications
- User activity tracking
- Conversations and messaging between users

## Features

### User Following
- Follow/unfollow other users
- View followers and following lists
- Check follow status
- Get suggested users to follow

### Notifications
- Real-time notifications using SignalR
- Create and manage notifications
- Mark notifications as read
- Get unread notification count

### Activities
- Track user activities across the platform
- View activity feed for followed users
- View global activity feed

### Conversations
- Create new conversations with multiple participants
- Add/remove participants
- View conversation history
- Manage conversation read status

### Messaging
- Send messages within conversations
- Real-time message delivery using SignalR
- Support for message attachments
- Mark messages as read

## Technical Implementation

### Architecture
- ASP.NET Core 6 Web API
- Entity Framework Core for data access
- SignalR for real-time communication
- JWT Authentication
- SQL Server database

### Key Components
- **Models**: Data models for follows, notifications, activities, conversations, and messages
- **DTOs**: Data transfer objects for API communication
- **Controllers**: RESTful API endpoints
- **Services**: Business logic implementation
- **Interfaces**: Service contracts for dependency injection
- **Hubs**: SignalR hubs for real-time communication
- **DbContext**: Entity Framework database context

### Real-time Communication
The microservice uses SignalR to implement real-time features:
- **NotificationHub**: For delivering real-time notifications
- **ConversationHub**: For real-time messaging and conversation updates

## API Endpoints

### Follows
- `POST /api/follows` - Follow a user
- `DELETE /api/follows/{followeeId}` - Unfollow a user
- `GET /api/follows/check/{followeeId}` - Check follow status
- `GET /api/follows/followers/{userId}` - Get user's followers
- `GET /api/follows/following/{userId}` - Get users being followed
- `GET /api/follows/suggestions` - Get suggested users to follow

### Notifications
- `POST /api/notifications` - Create a notification
- `GET /api/notifications` - Get user's notifications
- `GET /api/notifications/unread/count` - Get unread notification count
- `PUT /api/notifications/{notificationId}/read` - Mark notification as read
- `PUT /api/notifications/read/all` - Mark all notifications as read
- `DELETE /api/notifications/{notificationId}` - Delete a notification

### Activities
- `POST /api/activities` - Create an activity
- `GET /api/activities/user/{userId}` - Get user's activities
- `GET /api/activities/following` - Get activities from followed users
- `GET /api/activities` - Get global activities
- `DELETE /api/activities/{activityId}` - Delete an activity

### Conversations
- `POST /api/conversations` - Create a conversation
- `GET /api/conversations/{conversationId}` - Get conversation by ID
- `GET /api/conversations` - Get user's conversations
- `POST /api/conversations/{conversationId}/participants` - Add participant
- `DELETE /api/conversations/{conversationId}/participants/{userId}` - Remove participant
- `PUT /api/conversations/{conversationId}/read` - Mark conversation as read
- `GET /api/conversations/unread/count` - Get unread conversations count

### Messages
- `POST /api/conversations/{conversationId}/messages` - Create a message
- `GET /api/conversations/{conversationId}/messages` - Get conversation messages
- `PUT /api/messages/{messageId}/read` - Mark message as read
- `DELETE /api/messages/{messageId}` - Delete a message

## Configuration
The microservice is configured using appsettings.json and appsettings.Development.json:
- Database connection string
- JWT authentication settings
- Azure Service Bus connection
- Logging settings

## Setup Instructions
1. Make sure SQL Server is installed and running
2. Update connection strings in appsettings.json
3. Run Entity Framework migrations: `dotnet ef database update`
4. Run the application: `dotnet run`
