# Notifications API

The Notifications API handles user notification management, preferences, and delivery channels.

## Base URL

`/api/v1/notifications`

## Notification Endpoints

### Get User Notifications

Get notifications for the authenticated user.

```http
GET /api/v1/notifications?page=1&pageSize=20&unreadOnly=false
Authorization: Bearer your-jwt-token
```

**Query Parameters:**
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Number of items per page (default: 20, max: 100)
- `unreadOnly` (optional): Filter to only unread notifications (default: false)
- `types` (optional): Filter by notification types (comma-separated)

**Response:**

```json
{
  "items": [
    {
      "id": "d4e5f6g7-h8i9-j0k1-d4e5-f6g7h8i9j0k1",
      "type": "new_follower",
      "title": "New Follower",
      "message": "Jane Doe started following you",
      "read": false,
      "actor": {
        "id": "b3c5d7e9-f1a3-b5c7-d9e1-f3a5b7c9d1e3",
        "username": "janedoe",
        "avatarUrl": "https://storage.ouiai.com/avatars/janedoe.jpg"
      },
      "timestamp": "2023-05-10T14:25:30Z",
      "data": {
        "userId": "b3c5d7e9-f1a3-b5c7-d9e1-f3a5b7c9d1e3"
      }
    },
    {
      "id": "e5f6g7h8-i9j0-k1l2-e5f6-g7h8i9j0k1l2",
      "type": "comment_on_project",
      "title": "New Comment",
      "message": "Jane Doe commented on your project",
      "read": true,
      "actor": {
        "id": "b3c5d7e9-f1a3-b5c7-d9e1-f3a5b7c9d1e3",
        "username": "janedoe",
        "avatarUrl": "https://storage.ouiai.com/avatars/janedoe.jpg"
      },
      "timestamp": "2023-05-12T16:50:30Z",
      "data": {
        "projectId": "e5f6g7h8-i9j0-k1l2-m3n4-o5p6q7r8s9t0",
        "projectTitle": "Image Classification with Vision Transformer",
        "commentId": "c3d4e5f6-g7h8-i9j0-c3d4-e5f6g7h8i9j0",
        "commentPreview": "This is an excellent project! I particularly like..."
      }
    },
    {
      "id": "f6g7h8i9-j0k1-l2m3-f6g7-h8i9j0k1l2m3",
      "type": "project_liked",
      "title": "Project Liked",
      "message": "Jane Doe liked your project",
      "read": false,
      "actor": {
        "id": "b3c5d7e9-f1a3-b5c7-d9e1-f3a5b7c9d1e3",
        "username": "janedoe",
        "avatarUrl": "https://storage.ouiai.com/avatars/janedoe.jpg"
      },
      "timestamp": "2023-05-13T09:15:45Z",
      "data": {
        "projectId": "e5f6g7h8-i9j0-k1l2-m3n4-o5p6q7r8s9t0",
        "projectTitle": "Image Classification with Vision Transformer"
      }
    }
  ],
  "pagination": {
    "totalItems": 15,
    "totalPages": 1,
    "currentPage": 1,
    "pageSize": 20
  },
  "unreadCount": 2
}
```

**Status Codes:**
- `200 OK` - Notifications retrieved successfully

### Get Notification by ID

Get a specific notification by its ID.

```http
GET /api/v1/notifications/{notificationId}
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "id": "d4e5f6g7-h8i9-j0k1-d4e5-f6g7h8i9j0k1",
  "type": "new_follower",
  "title": "New Follower",
  "message": "Jane Doe started following you",
  "read": false,
  "actor": {
    "id": "b3c5d7e9-f1a3-b5c7-d9e1-f3a5b7c9d1e3",
    "username": "janedoe",
    "avatarUrl": "https://storage.ouiai.com/avatars/janedoe.jpg"
  },
  "timestamp": "2023-05-10T14:25:30Z",
  "data": {
    "userId": "b3c5d7e9-f1a3-b5c7-d9e1-f3a5b7c9d1e3"
  }
}
```

**Status Codes:**
- `200 OK` - Notification retrieved successfully
- `404 Not Found` - Notification not found

### Mark Notification as Read

Mark a single notification as read.

```http
PUT /api/v1/notifications/{notificationId}/read
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "id": "d4e5f6g7-h8i9-j0k1-d4e5-f6g7h8i9j0k1",
  "read": true
}
```

**Status Codes:**
- `200 OK` - Notification marked as read
- `404 Not Found` - Notification not found

### Mark Notification as Unread

Mark a single notification as unread.

```http
PUT /api/v1/notifications/{notificationId}/unread
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "id": "d4e5f6g7-h8i9-j0k1-d4e5-f6g7h8i9j0k1",
  "read": false
}
```

**Status Codes:**
- `200 OK` - Notification marked as unread
- `404 Not Found` - Notification not found

### Mark All Notifications as Read

Mark all notifications as read.

```http
PUT /api/v1/notifications/read-all
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "message": "All notifications marked as read",
  "unreadCount": 0
}
```

**Status Codes:**
- `200 OK` - All notifications marked as read

### Delete Notification

Delete a specific notification.

```http
DELETE /api/v1/notifications/{notificationId}
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "message": "Notification deleted successfully"
}
```

**Status Codes:**
- `200 OK` - Notification deleted successfully
- `404 Not Found` - Notification not found

### Delete All Notifications

Delete all notifications for the current user.

```http
DELETE /api/v1/notifications
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "message": "All notifications deleted successfully"
}
```

**Status Codes:**
- `200 OK` - All notifications deleted successfully

## Notification Preferences Endpoints

### Get Notification Preferences

Get the notification preferences for the authenticated user.

```http
GET /api/v1/notifications/preferences
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "email": {
    "new_follower": true,
    "comment_on_project": true,
    "project_liked": false,
    "mention": true,
    "new_version": true,
    "system_announcement": true
  },
  "push": {
    "new_follower": true,
    "comment_on_project": true,
    "project_liked": true,
    "mention": true,
    "new_version": false,
    "system_announcement": true
  },
  "inApp": {
    "new_follower": true,
    "comment_on_project": true,
    "project_liked": true,
    "mention": true,
    "new_version": true,
    "system_announcement": true
  }
}
```

**Status Codes:**
- `200 OK` - Preferences retrieved successfully

### Update Notification Preferences

Update notification preferences for the authenticated user.

```http
PUT /api/v1/notifications/preferences
Authorization: Bearer your-jwt-token
Content-Type: application/json

{
  "email": {
    "new_follower": true,
    "comment_on_project": true,
    "project_liked": false,
    "mention": true,
    "new_version": true,
    "system_announcement": true
  },
  "push": {
    "new_follower": true,
    "comment_on_project": true,
    "project_liked": false,
    "mention": true,
    "new_version": false,
    "system_announcement": true
  },
  "inApp": {
    "new_follower": true,
    "comment_on_project": true,
    "project_liked": true,
    "mention": true,
    "new_version": true,
    "system_announcement": true
  }
}
```

**Response:**

```json
{
  "email": {
    "new_follower": true,
    "comment_on_project": true,
    "project_liked": false,
    "mention": true,
    "new_version": true,
    "system_announcement": true
  },
  "push": {
    "new_follower": true,
    "comment_on_project": true,
    "project_liked": false,
    "mention": true,
    "new_version": false,
    "system_announcement": true
  },
  "inApp": {
    "new_follower": true,
    "comment_on_project": true,
    "project_liked": true,
    "mention": true,
    "new_version": true,
    "system_announcement": true
  }
}
```

**Status Codes:**
- `200 OK` - Preferences updated successfully
- `400 Bad Request` - Invalid preferences data

### Update Channel Preferences

Update preferences for a specific notification channel.

```http
PUT /api/v1/notifications/preferences/{channel}
Authorization: Bearer your-jwt-token
Content-Type: application/json

{
  "new_follower": true,
  "comment_on_project": true,
  "project_liked": true,
  "mention": true,
  "new_version": false,
  "system_announcement": true
}
```

**Path Parameters:**
- `channel`: Notification channel (email, push, inApp)

**Response:**

```json
{
  "new_follower": true,
  "comment_on_project": true,
  "project_liked": true,
  "mention": true,
  "new_version": false,
  "system_announcement": true
}
```

**Status Codes:**
- `200 OK` - Channel preferences updated successfully
- `400 Bad Request` - Invalid preferences data or invalid channel

## Push Notification Endpoints

### Register Push Device

Register a device for push notifications.

```http
POST /api/v1/notifications/devices
Authorization: Bearer your-jwt-token
Content-Type: application/json

{
  "token": "fcm-device-token-here",
  "platform": "android",
  "appVersion": "1.2.0"
}
```

**Response:**

```json
{
  "id": "g7h8i9j0-k1l2-m3n4-g7h8-i9j0k1l2m3n4",
  "token": "fcm-device-token-here",
  "platform": "android",
  "appVersion": "1.2.0",
  "createdAt": "2023-05-20T09:30:00Z",
  "lastActiveAt": "2023-05-20T09:30:00Z"
}
```

**Status Codes:**
- `201 Created` - Device registered successfully
- `400 Bad Request` - Invalid device data

### Update Push Device

Update a registered push notification device.

```http
PUT /api/v1/notifications/devices/{deviceId}
Authorization: Bearer your-jwt-token
Content-Type: application/json

{
  "token": "new-fcm-device-token-here",
  "appVersion": "1.3.0"
}
```

**Response:**

```json
{
  "id": "g7h8i9j0-k1l2-m3n4-g7h8-i9j0k1l2m3n4",
  "token": "new-fcm-device-token-here",
  "platform": "android",
  "appVersion": "1.3.0",
  "createdAt": "2023-05-20T09:30:00Z",
  "lastActiveAt": "2023-05-21T14:45:00Z"
}
```

**Status Codes:**
- `200 OK` - Device updated successfully
- `400 Bad Request` - Invalid device data
- `404 Not Found` - Device not found

### Delete Push Device

Remove a device from push notifications.

```http
DELETE /api/v1/notifications/devices/{deviceId}
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "message": "Device removed successfully"
}
```

**Status Codes:**
- `200 OK` - Device removed successfully
- `404 Not Found` - Device not found

### Get User Devices

Get all registered devices for the authenticated user.

```http
GET /api/v1/notifications/devices
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "items": [
    {
      "id": "g7h8i9j0-k1l2-m3n4-g7h8-i9j0k1l2m3n4",
      "platform": "android",
      "appVersion": "1.3.0",
      "createdAt": "2023-05-20T09:30:00Z",
      "lastActiveAt": "2023-05-21T14:45:00Z"
    },
    {
      "id": "h8i9j0k1-l2m3-n4o5-h8i9-j0k1l2m3n4o5",
      "platform": "ios",
      "appVersion": "1.2.5",
      "createdAt": "2023-04-15T11:20:00Z",
      "lastActiveAt": "2023-05-19T16:30:00Z"
    }
  ]
}
```

**Status Codes:**
- `200 OK` - Devices retrieved successfully

## Email Notification Endpoints

### Verify Email for Notifications

Verify the user's email for receiving notifications.

```http
POST /api/v1/notifications/email/verify
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "message": "Verification email sent successfully"
}
```

**Status Codes:**
- `200 OK` - Verification email sent successfully

### Confirm Email Verification

Confirm the email verification using a token.

```http
GET /api/v1/notifications/email/confirm?token=verification-token-here
```

**Query Parameters:**
- `token`: Verification token from the email

**Response:**

```json
{
  "message": "Email verification successful",
  "verified": true
}
```

**Status Codes:**
- `200 OK` - Email verified successfully
- `400 Bad Request` - Invalid or expired token

### Unsubscribe from Email Notifications

Unsubscribe from a specific type of email notification.

```http
GET /api/v1/notifications/email/unsubscribe?token=unsubscribe-token-here&type=all
```

**Query Parameters:**
- `token`: Unsubscribe token from the email
- `type`: Notification type to unsubscribe from (or 'all' for all types)

**Response:**

```json
{
  "message": "Successfully unsubscribed from all email notifications",
  "preferences": {
    "email": {
      "new_follower": false,
      "comment_on_project": false,
      "project_liked": false,
      "mention": false,
      "new_version": false,
      "system_announcement": false
    }
  }
}
```

**Status Codes:**
- `200 OK` - Unsubscribed successfully
- `400 Bad Request` - Invalid token

## Notification Types

The system supports the following notification types:

| Type | Description |
|------|-------------|
| new_follower | Someone followed the user |
| comment_on_project | Someone commented on the user's project |
| project_liked | Someone liked the user's project |
| mention | Someone mentioned the user in a comment |
| new_version | A project the user follows has a new version |
| system_announcement | System-wide announcement |

## Error Responses

### Notification Not Found

```json
{
  "status": 404,
  "message": "Notification with ID 'd4e5f6g7-h8i9-j0k1-d4e5-f6g7h8i9j0k1' not found.",
  "errors": null,
  "traceId": "7f8d3a2e-1c5b-4d9a-8f7e-3a2c1d9b5a7e"
}
```

### Invalid Notification Preferences

```json
{
  "status": 400,
  "message": "Validation failed.",
  "errors": {
    "email.new_follower": ["The field must be a boolean value."]
  },
  "traceId": "7f8d3a2e-1c5b-4d9a-8f7e-3a2c1d9b5a7e"
}
```

### Invalid Device Data

```json
{
  "status": 400,
  "message": "Validation failed.",
  "errors": {
    "Token": ["The Token field is required."],
    "Platform": ["Platform must be one of: android, ios, web."]
  },
  "traceId": "7f8d3a2e-1c5b-4d9a-8f7e-3a2c1d9b5a7e"
}
```

## Data Models

### NotificationDto

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| id | string | Yes | Notification unique identifier |
| type | string | Yes | Notification type |
| title | string | Yes | Short notification title |
| message | string | Yes | Notification message |
| read | boolean | Yes | Whether notification has been read |
| actor | UserDto | No | User who triggered the notification (if applicable) |
| timestamp | datetime | Yes | Notification creation timestamp |
| data | object | No | Additional notification data |

### NotificationPreferencesDto

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| email | object | Yes | Email notification preferences |
| push | object | Yes | Push notification preferences |
| inApp | object | Yes | In-app notification preferences |

### DeviceRegistrationDto

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| token | string | Yes | Device push token |
| platform | string | Yes | Device platform (android, ios, web) |
| appVersion | string | Yes | App version running on the device |

### DeviceDto

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| id | string | Yes | Device unique identifier |
| platform | string | Yes | Device platform |
| appVersion | string | Yes | App version |
| createdAt | datetime | Yes | When the device was registered |
| lastActiveAt | datetime | Yes | When the device was last active |
