# Social API

The Social API handles social interactions between users, including following relationships, activity feeds, comments, and likes.

## Base URL

`/api/v1/social`

## Following Endpoints

### Follow a User

Follow another user.

```http
POST /api/v1/social/follow/{userId}
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "following": true,
  "followedAt": "2023-05-10T14:25:30Z"
}
```

**Status Codes:**
- `200 OK` - Successfully followed the user
- `400 Bad Request` - Cannot follow yourself
- `404 Not Found` - User not found

### Unfollow a User

Unfollow a user you are currently following.

```http
DELETE /api/v1/social/follow/{userId}
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "following": false
}
```

**Status Codes:**
- `200 OK` - Successfully unfollowed the user
- `404 Not Found` - User not found or not following

### Get User Followers

Get users who are following a specific user.

```http
GET /api/v1/social/followers/{userId}?page=1&pageSize=20
Authorization: Bearer your-jwt-token
```

**Query Parameters:**
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Number of items per page (default: 20, max: 100)

**Response:**

```json
{
  "items": [
    {
      "id": "b3c5d7e9-f1a3-b5c7-d9e1-f3a5b7c9d1e3",
      "username": "janedoe",
      "firstName": "Jane",
      "lastName": "Doe",
      "avatarUrl": "https://storage.ouiai.com/avatars/janedoe.jpg",
      "followingSince": "2023-05-10T14:25:30Z",
      "isFollowingBack": true
    },
    // More followers...
  ],
  "pagination": {
    "totalItems": 120,
    "totalPages": 6,
    "currentPage": 1,
    "pageSize": 20
  }
}
```

**Status Codes:**
- `200 OK` - Successfully retrieved followers
- `404 Not Found` - User not found

### Get User Following

Get users that a specific user is following.

```http
GET /api/v1/social/following/{userId}?page=1&pageSize=20
Authorization: Bearer your-jwt-token
```

**Query Parameters:**
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Number of items per page (default: 20, max: 100)

**Response:**

```json
{
  "items": [
    {
      "id": "f7e3a9d1-b2c5-4d8e-a1f7-e9b2c5d8e1a7",
      "username": "johndoe",
      "firstName": "John",
      "lastName": "Doe",
      "avatarUrl": "https://storage.ouiai.com/avatars/johndoe.jpg",
      "followingSince": "2023-04-15T09:30:00Z",
      "isFollowingBack": true
    },
    // More following users...
  ],
  "pagination": {
    "totalItems": 85,
    "totalPages": 5,
    "currentPage": 1,
    "pageSize": 20
  }
}
```

**Status Codes:**
- `200 OK` - Successfully retrieved following users
- `404 Not Found` - User not found

### Check Following Status

Check if you are following a specific user.

```http
GET /api/v1/social/follow/{userId}/status
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "following": true,
  "followedAt": "2023-05-10T14:25:30Z",
  "isFollowingBack": false
}
```

**Status Codes:**
- `200 OK` - Successfully retrieved following status
- `404 Not Found` - User not found

## Activity Feed Endpoints

### Get User Activity Feed

Get the current user's activity feed.

```http
GET /api/v1/social/feed?page=1&pageSize=20
Authorization: Bearer your-jwt-token
```

**Query Parameters:**
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Number of items per page (default: 20, max: 100)

**Response:**

```json
{
  "items": [
    {
      "id": "a1b2c3d4-e5f6-7890-a1b2-c3d4e5f67890",
      "type": "project_created",
      "actor": {
        "id": "f7e3a9d1-b2c5-4d8e-a1f7-e9b2c5d8e1a7",
        "username": "johndoe",
        "avatarUrl": "https://storage.ouiai.com/avatars/johndoe.jpg"
      },
      "project": {
        "id": "e5f6g7h8-i9j0-k1l2-m3n4-o5p6q7r8s9t0",
        "title": "Image Classification with Vision Transformer",
        "thumbnailUrl": "https://storage.ouiai.com/projects/thumbnails/medical-vit.jpg"
      },
      "timestamp": "2023-05-12T16:45:30Z"
    },
    {
      "id": "b2c3d4e5-f6g7-8901-b2c3-d4e5f6g78901",
      "type": "user_followed",
      "actor": {
        "id": "b3c5d7e9-f1a3-b5c7-d9e1-f3a5b7c9d1e3",
        "username": "janedoe",
        "avatarUrl": "https://storage.ouiai.com/avatars/janedoe.jpg"
      },
      "target": {
        "id": "f7e3a9d1-b2c5-4d8e-a1f7-e9b2c5d8e1a7",
        "username": "johndoe",
        "avatarUrl": "https://storage.ouiai.com/avatars/johndoe.jpg"
      },
      "timestamp": "2023-05-10T14:25:30Z"
    },
    // More activity items...
  ],
  "pagination": {
    "totalItems": 157,
    "totalPages": 8,
    "currentPage": 1,
    "pageSize": 20
  }
}
```

**Status Codes:**
- `200 OK` - Successfully retrieved activity feed

### Get User Profile Activity

Get activity for a specific user's profile.

```http
GET /api/v1/social/users/{userId}/activity?page=1&pageSize=20
Authorization: Bearer your-jwt-token
```

**Query Parameters:**
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Number of items per page (default: 20, max: 100)

**Response:**

```json
{
  "items": [
    {
      "id": "a1b2c3d4-e5f6-7890-a1b2-c3d4e5f67890",
      "type": "project_created",
      "actor": {
        "id": "f7e3a9d1-b2c5-4d8e-a1f7-e9b2c5d8e1a7",
        "username": "johndoe",
        "avatarUrl": "https://storage.ouiai.com/avatars/johndoe.jpg"
      },
      "project": {
        "id": "e5f6g7h8-i9j0-k1l2-m3n4-o5p6q7r8s9t0",
        "title": "Image Classification with Vision Transformer",
        "thumbnailUrl": "https://storage.ouiai.com/projects/thumbnails/medical-vit.jpg"
      },
      "timestamp": "2023-05-12T16:45:30Z"
    },
    // More activity items...
  ],
  "pagination": {
    "totalItems": 42,
    "totalPages": 3,
    "currentPage": 1,
    "pageSize": 20
  }
}
```

**Status Codes:**
- `200 OK` - Successfully retrieved user activity
- `404 Not Found` - User not found

## Comments Endpoints

### Add Comment to Project

Add a comment to a project.

```http
POST /api/v1/social/projects/{projectId}/comments
Authorization: Bearer your-jwt-token
Content-Type: application/json

{
  "content": "This is an excellent project! I particularly like the approach to handling medical images."
}
```

**Response:**

```json
{
  "id": "c3d4e5f6-g7h8-i9j0-c3d4-e5f6g7h8i9j0",
  "content": "This is an excellent project! I particularly like the approach to handling medical images.",
  "user": {
    "id": "f7e3a9d1-b2c5-4d8e-a1f7-e9b2c5d8e1a7",
    "username": "johndoe",
    "avatarUrl": "https://storage.ouiai.com/avatars/johndoe.jpg"
  },
  "createdAt": "2023-05-15T10:20:30Z",
  "updatedAt": null,
  "likesCount": 0,
  "isLiked": false
}
```

**Status Codes:**
- `201 Created` - Comment created successfully
- `400 Bad Request` - Invalid comment data
- `404 Not Found` - Project not found

### Get Project Comments

Get comments for a specific project.

```http
GET /api/v1/social/projects/{projectId}/comments?page=1&pageSize=20&sort=newest
Authorization: Bearer your-jwt-token
```

**Query Parameters:**
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Number of items per page (default: 20, max: 100)
- `sort` (optional): Sort order (newest, oldest, mostLiked)

**Response:**

```json
{
  "items": [
    {
      "id": "c3d4e5f6-g7h8-i9j0-c3d4-e5f6g7h8i9j0",
      "content": "This is an excellent project! I particularly like the approach to handling medical images.",
      "user": {
        "id": "f7e3a9d1-b2c5-4d8e-a1f7-e9b2c5d8e1a7",
        "username": "johndoe",
        "avatarUrl": "https://storage.ouiai.com/avatars/johndoe.jpg"
      },
      "createdAt": "2023-05-15T10:20:30Z",
      "updatedAt": null,
      "likesCount": 3,
      "isLiked": true
    },
    // More comments...
  ],
  "pagination": {
    "totalItems": 8,
    "totalPages": 1,
    "currentPage": 1,
    "pageSize": 20
  }
}
```

**Status Codes:**
- `200 OK` - Comments retrieved successfully
- `404 Not Found` - Project not found

### Update Comment

Update an existing comment.

```http
PUT /api/v1/social/comments/{commentId}
Authorization: Bearer your-jwt-token
Content-Type: application/json

{
  "content": "This is an excellent project! I particularly like the approach to handling medical images and the performance optimizations."
}
```

**Response:**

```json
{
  "id": "c3d4e5f6-g7h8-i9j0-c3d4-e5f6g7h8i9j0",
  "content": "This is an excellent project! I particularly like the approach to handling medical images and the performance optimizations.",
  "user": {
    "id": "f7e3a9d1-b2c5-4d8e-a1f7-e9b2c5d8e1a7",
    "username": "johndoe",
    "avatarUrl": "https://storage.ouiai.com/avatars/johndoe.jpg"
  },
  "createdAt": "2023-05-15T10:20:30Z",
  "updatedAt": "2023-05-15T10:30:45Z",
  "likesCount": 3,
  "isLiked": true
}
```

**Status Codes:**
- `200 OK` - Comment updated successfully
- `400 Bad Request` - Invalid comment data
- `403 Forbidden` - Not authorized to update the comment
- `404 Not Found` - Comment not found

### Delete Comment

Delete a comment.

```http
DELETE /api/v1/social/comments/{commentId}
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "message": "Comment deleted successfully"
}
```

**Status Codes:**
- `200 OK` - Comment deleted successfully
- `403 Forbidden` - Not authorized to delete the comment
- `404 Not Found` - Comment not found

### Like Comment

Like a comment.

```http
POST /api/v1/social/comments/{commentId}/like
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "likesCount": 4,
  "isLiked": true
}
```

**Status Codes:**
- `200 OK` - Comment liked successfully
- `404 Not Found` - Comment not found

### Unlike Comment

Remove a like from a comment.

```http
DELETE /api/v1/social/comments/{commentId}/like
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "likesCount": 3,
  "isLiked": false
}
```

**Status Codes:**
- `200 OK` - Comment unliked successfully
- `404 Not Found` - Comment not found

## Notifications Endpoints

### Get User Notifications

Get notifications for the current user.

```http
GET /api/v1/social/notifications?page=1&pageSize=20&unreadOnly=false
Authorization: Bearer your-jwt-token
```

**Query Parameters:**
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Number of items per page (default: 20, max: 100)
- `unreadOnly` (optional): Filter to only unread notifications (default: false)

**Response:**

```json
{
  "items": [
    {
      "id": "d4e5f6g7-h8i9-j0k1-d4e5-f6g7h8i9j0k1",
      "type": "new_follower",
      "read": false,
      "actor": {
        "id": "b3c5d7e9-f1a3-b5c7-d9e1-f3a5b7c9d1e3",
        "username": "janedoe",
        "avatarUrl": "https://storage.ouiai.com/avatars/janedoe.jpg"
      },
      "timestamp": "2023-05-10T14:25:30Z",
      "data": {
        "message": "Jane Doe started following you"
      }
    },
    {
      "id": "e5f6g7h8-i9j0-k1l2-e5f6-g7h8i9j0k1l2",
      "type": "comment_on_project",
      "read": true,
      "actor": {
        "id": "b3c5d7e9-f1a3-b5c7-d9e1-f3a5b7c9d1e3",
        "username": "janedoe",
        "avatarUrl": "https://storage.ouiai.com/avatars/janedoe.jpg"
      },
      "timestamp": "2023-05-12T16:50:30Z",
      "data": {
        "message": "Jane Doe commented on your project",
        "projectId": "e5f6g7h8-i9j0-k1l2-m3n4-o5p6q7r8s9t0",
        "projectTitle": "Image Classification with Vision Transformer",
        "commentId": "c3d4e5f6-g7h8-i9j0-c3d4-e5f6g7h8i9j0",
        "commentPreview": "This is an excellent project! I particularly like..."
      }
    },
    // More notifications...
  ],
  "pagination": {
    "totalItems": 15,
    "totalPages": 1,
    "currentPage": 1,
    "pageSize": 20
  },
  "unreadCount": 3
}
```

**Status Codes:**
- `200 OK` - Notifications retrieved successfully

### Mark Notification as Read

Mark a notification as read.

```http
PUT /api/v1/social/notifications/{notificationId}/read
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "read": true
}
```

**Status Codes:**
- `200 OK` - Notification marked as read
- `404 Not Found` - Notification not found

### Mark All Notifications as Read

Mark all notifications as read.

```http
PUT /api/v1/social/notifications/read-all
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

## Error Responses

### Comment Not Found

```json
{
  "status": 404,
  "message": "Comment with ID 'c3d4e5f6-g7h8-i9j0-c3d4-e5f6g7h8i9j0' not found.",
  "errors": null,
  "traceId": "7f8d3a2e-1c5b-4d9a-8f7e-3a2c1d9b5a7e"
}
```

### Unauthorized Comment Action

```json
{
  "status": 403,
  "message": "You do not have permission to update this comment.",
  "errors": null,
  "traceId": "7f8d3a2e-1c5b-4d9a-8f7e-3a2c1d9b5a7e"
}
```

### Invalid Comment Data

```json
{
  "status": 400,
  "message": "Validation failed.",
  "errors": {
    "Content": ["The Content field is required."]
  },
  "traceId": "7f8d3a2e-1c5b-4d9a-8f7e-3a2c1d9b5a7e"
}
```

## Data Models

### CommentCreateDto

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| content | string | Yes | Comment text content |

### CommentUpdateDto

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| content | string | Yes | Updated comment text content |

### CommentDto

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| id | string | Yes | Comment unique identifier |
| content | string | Yes | Comment text content |
| user | UserDto | Yes | User who created the comment |
| createdAt | datetime | Yes | Comment creation timestamp |
| updatedAt | datetime | No | Comment last update timestamp |
| likesCount | integer | Yes | Number of likes on the comment |
| isLiked | boolean | Yes | Whether current user liked the comment |

### NotificationDto

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| id | string | Yes | Notification unique identifier |
| type | string | Yes | Notification type |
| read | boolean | Yes | Whether notification has been read |
| actor | UserDto | Yes | User who triggered the notification |
| timestamp | datetime | Yes | Notification creation timestamp |
| data | object | Yes | Additional notification data |
