# Identity API

The Identity API handles user authentication, registration, and profile management.

## Base URL

`/api/v1/identity`

## Authentication Endpoints

### Register a new user

```http
POST /api/v1/identity/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123",
  "confirmPassword": "password123",
  "firstName": "John",
  "lastName": "Doe",
  "username": "johndoe"
}
```

**Response**:

```json
{
  "id": "f7e3a9d1-b2c5-4d8e-a1f7-e9b2c5d8e1a7",
  "email": "user@example.com",
  "username": "johndoe",
  "firstName": "John",
  "lastName": "Doe",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Status Codes**:
- `201 Created` - User created successfully
- `400 Bad Request` - Invalid request data

### Login

```http
POST /api/v1/identity/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response**:

```json
{
  "id": "f7e3a9d1-b2c5-4d8e-a1f7-e9b2c5d8e1a7",
  "email": "user@example.com",
  "username": "johndoe",
  "firstName": "John",
  "lastName": "Doe",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Status Codes**:
- `200 OK` - Login successful
- `401 Unauthorized` - Invalid credentials

### Request Password Reset

```http
POST /api/v1/identity/forgot-password
Content-Type: application/json

{
  "email": "user@example.com"
}
```

**Response**:

```json
{
  "message": "If your email exists in our system, you will receive a password reset link."
}
```

**Status Codes**:
- `200 OK` - Request processed

### Reset Password

```http
POST /api/v1/identity/reset-password
Content-Type: application/json

{
  "email": "user@example.com",
  "token": "reset-token-from-email",
  "newPassword": "newPassword123",
  "confirmPassword": "newPassword123"
}
```

**Response**:

```json
{
  "message": "Password has been reset successfully."
}
```

**Status Codes**:
- `200 OK` - Password reset successful
- `400 Bad Request` - Invalid token or passwords don't match

### Refresh Token

```http
POST /api/v1/identity/refresh-token
Content-Type: application/json

{
  "refreshToken": "your-refresh-token"
}
```

**Response**:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "new-refresh-token"
}
```

**Status Codes**:
- `200 OK` - Token refreshed successfully
- `401 Unauthorized` - Invalid refresh token

### Logout

```http
POST /api/v1/identity/logout
Authorization: Bearer your-jwt-token
```

**Response**:

```json
{
  "message": "Logged out successfully"
}
```

**Status Codes**:
- `200 OK` - Logout successful

## User Profile Endpoints

### Get Current User Profile

```http
GET /api/v1/identity/profile
Authorization: Bearer your-jwt-token
```

**Response**:

```json
{
  "id": "f7e3a9d1-b2c5-4d8e-a1f7-e9b2c5d8e1a7",
  "email": "user@example.com",
  "username": "johndoe",
  "firstName": "John",
  "lastName": "Doe",
  "bio": "AI enthusiast and developer",
  "avatarUrl": "https://storage.ouiai.com/avatars/johndoe.jpg",
  "createdAt": "2023-01-15T08:30:00Z",
  "updatedAt": "2023-02-20T14:15:30Z",
  "followersCount": 42,
  "followingCount": 56
}
```

**Status Codes**:
- `200 OK` - Profile retrieved successfully
- `401 Unauthorized` - Not authenticated

### Update User Profile

```http
PUT /api/v1/identity/profile
Authorization: Bearer your-jwt-token
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "bio": "AI enthusiast and developer",
  "username": "johndoe"
}
```

**Response**:

```json
{
  "id": "f7e3a9d1-b2c5-4d8e-a1f7-e9b2c5d8e1a7",
  "email": "user@example.com",
  "username": "johndoe",
  "firstName": "John",
  "lastName": "Doe",
  "bio": "AI enthusiast and developer",
  "avatarUrl": "https://storage.ouiai.com/avatars/johndoe.jpg",
  "updatedAt": "2023-03-10T09:45:12Z"
}
```

**Status Codes**:
- `200 OK` - Profile updated successfully
- `400 Bad Request` - Invalid data

### Change Password

```http
PUT /api/v1/identity/change-password
Authorization: Bearer your-jwt-token
Content-Type: application/json

{
  "currentPassword": "password123",
  "newPassword": "newPassword456",
  "confirmPassword": "newPassword456"
}
```

**Response**:

```json
{
  "message": "Password changed successfully"
}
```

**Status Codes**:
- `200 OK` - Password changed successfully
- `400 Bad Request` - Invalid passwords
- `401 Unauthorized` - Current password is incorrect

### Upload Profile Picture

```http
POST /api/v1/identity/profile/picture
Authorization: Bearer your-jwt-token
Content-Type: multipart/form-data

file: [binary data]
```

**Response**:

```json
{
  "avatarUrl": "https://storage.ouiai.com/avatars/johndoe-new.jpg"
}
```

**Status Codes**:
- `200 OK` - Picture uploaded successfully
- `400 Bad Request` - Invalid file format or size

## User Management Endpoints

### Get User by ID

```http
GET /api/v1/identity/users/{userId}
Authorization: Bearer your-jwt-token
```

**Response**:

```json
{
  "id": "a1b2c3d4-e5f6-7890-a1b2-c3d4e5f67890",
  "username": "janedoe",
  "firstName": "Jane",
  "lastName": "Doe",
  "bio": "AI researcher and data scientist",
  "avatarUrl": "https://storage.ouiai.com/avatars/janedoe.jpg",
  "createdAt": "2023-01-10T10:20:30Z",
  "followersCount": 120,
  "followingCount": 85,
  "isFollowing": true
}
```

**Status Codes**:
- `200 OK` - User found
- `404 Not Found` - User not found

### Search Users

```http
GET /api/v1/identity/users/search?query=john&page=1&pageSize=20
Authorization: Bearer your-jwt-token
```

**Response**:

```json
{
  "items": [
    {
      "id": "f7e3a9d1-b2c5-4d8e-a1f7-e9b2c5d8e1a7",
      "username": "johndoe",
      "firstName": "John",
      "lastName": "Doe",
      "avatarUrl": "https://storage.ouiai.com/avatars/johndoe.jpg",
      "isFollowing": false
    },
    {
      "id": "b3c5d7e9-f1a3-b5c7-d9e1-f3a5b7c9d1e3",
      "username": "johnsmith",
      "firstName": "John",
      "lastName": "Smith",
      "avatarUrl": "https://storage.ouiai.com/avatars/johnsmith.jpg",
      "isFollowing": true
    }
  ],
  "pagination": {
    "totalItems": 12,
    "totalPages": 1,
    "currentPage": 1,
    "pageSize": 20
  }
}
```

**Status Codes**:
- `200 OK` - Search completed

## Error Responses

### Invalid Credentials

```json
{
  "status": 401,
  "message": "Invalid email or password.",
  "errors": null,
  "traceId": "7f8d3a2e-1c5b-4d9a-8f7e-3a2c1d9b5a7e"
}
```

### Registration Validation Error

```json
{
  "status": 400,
  "message": "Validation failed.",
  "errors": {
    "Email": ["The Email field is not a valid e-mail address."],
    "Password": ["Passwords must be at least 8 characters."]
  },
  "traceId": "7f8d3a2e-1c5b-4d9a-8f7e-3a2c1d9b5a7e"
}
```

## Data Models

### UserRegisterDto

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| email | string | Yes | User's email address |
| password | string | Yes | User's password (min 8 chars) |
| confirmPassword | string | Yes | Confirmation of password |
| firstName | string | Yes | User's first name |
| lastName | string | Yes | User's last name |
| username | string | Yes | User's unique username |

### UserLoginDto

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| email | string | Yes | User's email address |
| password | string | Yes | User's password |

### UserProfileDto

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| id | string | Yes | User's unique identifier |
| email | string | Yes | User's email address |
| username | string | Yes | User's username |
| firstName | string | Yes | User's first name |
| lastName | string | Yes | User's last name |
| bio | string | No | User's biography |
| avatarUrl | string | No | URL to user's avatar |
| createdAt | datetime | Yes | Account creation timestamp |
| updatedAt | datetime | Yes | Last update timestamp |
| followersCount | integer | Yes | Number of followers |
| followingCount | integer | Yes | Number of users being followed |
