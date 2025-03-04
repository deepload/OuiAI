# Projects API

The Projects API handles AI project management, including creation, updating, categorization, and collaboration.

## Base URL

`/api/v1/projects`

## Project Endpoints

### Get All Projects

Get a paginated list of projects.

```http
GET /api/v1/projects?page=1&pageSize=20&filter.category=NLP&sort=-createdAt
Authorization: Bearer your-jwt-token
```

**Query Parameters:**
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Number of items per page (default: 20, max: 100)
- `filter.category` (optional): Filter by category
- `filter.tags` (optional): Filter by tags (comma-separated)
- `filter.userId` (optional): Filter by user ID
- `sort` (optional): Sort field (prefix with '-' for descending)

**Response:**

```json
{
  "items": [
    {
      "id": "a1b2c3d4-e5f6-7890-a1b2-c3d4e5f67890",
      "title": "Sentiment Analysis with BERT",
      "description": "A sentiment analysis model using BERT for social media texts",
      "thumbnailUrl": "https://storage.ouiai.com/projects/thumbnails/sentiment-bert.jpg",
      "category": "NLP",
      "tags": ["BERT", "Sentiment Analysis", "NLP", "Social Media"],
      "userId": "f7e3a9d1-b2c5-4d8e-a1f7-e9b2c5d8e1a7",
      "username": "johndoe",
      "userAvatarUrl": "https://storage.ouiai.com/avatars/johndoe.jpg",
      "createdAt": "2023-03-10T15:30:00Z",
      "updatedAt": "2023-03-15T09:45:30Z",
      "likesCount": 42,
      "commentsCount": 8,
      "isLiked": true
    },
    // More projects...
  ],
  "pagination": {
    "totalItems": 86,
    "totalPages": 5,
    "currentPage": 1,
    "pageSize": 20
  }
}
```

**Status Codes:**
- `200 OK` - Projects retrieved successfully

### Get Project by ID

Get detailed information about a specific project.

```http
GET /api/v1/projects/{projectId}
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "id": "a1b2c3d4-e5f6-7890-a1b2-c3d4e5f67890",
  "title": "Sentiment Analysis with BERT",
  "description": "A sentiment analysis model using BERT for social media texts",
  "fullDescription": "# Sentiment Analysis with BERT\n\nThis project implements a fine-tuned BERT model for sentiment analysis on social media data...",
  "thumbnailUrl": "https://storage.ouiai.com/projects/thumbnails/sentiment-bert.jpg",
  "category": "NLP",
  "tags": ["BERT", "Sentiment Analysis", "NLP", "Social Media"],
  "userId": "f7e3a9d1-b2c5-4d8e-a1f7-e9b2c5d8e1a7",
  "username": "johndoe",
  "userAvatarUrl": "https://storage.ouiai.com/avatars/johndoe.jpg",
  "createdAt": "2023-03-10T15:30:00Z",
  "updatedAt": "2023-03-15T09:45:30Z",
  "likesCount": 42,
  "commentsCount": 8,
  "isLiked": true,
  "isBookmarked": false,
  "visibility": "public",
  "license": "MIT",
  "repositoryUrl": "https://github.com/johndoe/sentiment-bert",
  "demoUrl": "https://demo.ouiai.com/johndoe/sentiment-bert",
  "collaborators": [
    {
      "userId": "b3c5d7e9-f1a3-b5c7-d9e1-f3a5b7c9d1e3",
      "username": "janedoe",
      "avatarUrl": "https://storage.ouiai.com/avatars/janedoe.jpg",
      "role": "contributor"
    }
  ],
  "versions": [
    {
      "version": "1.0.0",
      "description": "Initial release",
      "createdAt": "2023-03-10T15:30:00Z"
    },
    {
      "version": "1.1.0",
      "description": "Improved accuracy by 5%",
      "createdAt": "2023-03-15T09:45:30Z"
    }
  ]
}
```

**Status Codes:**
- `200 OK` - Project retrieved successfully
- `404 Not Found` - Project not found

### Create Project

Create a new AI project.

```http
POST /api/v1/projects
Authorization: Bearer your-jwt-token
Content-Type: application/json

{
  "title": "Image Classification with Vision Transformer",
  "description": "A ViT-based image classification model for medical imaging",
  "fullDescription": "# Image Classification with Vision Transformer\n\nThis project implements...",
  "category": "Computer Vision",
  "tags": ["Vision Transformer", "Medical Imaging", "Image Classification"],
  "visibility": "public",
  "license": "MIT",
  "repositoryUrl": "https://github.com/johndoe/medical-vit"
}
```

**Response:**

```json
{
  "id": "e5f6g7h8-i9j0-k1l2-m3n4-o5p6q7r8s9t0",
  "title": "Image Classification with Vision Transformer",
  "description": "A ViT-based image classification model for medical imaging",
  "fullDescription": "# Image Classification with Vision Transformer\n\nThis project implements...",
  "category": "Computer Vision",
  "tags": ["Vision Transformer", "Medical Imaging", "Image Classification"],
  "userId": "f7e3a9d1-b2c5-4d8e-a1f7-e9b2c5d8e1a7",
  "createdAt": "2023-04-05T11:20:00Z",
  "updatedAt": "2023-04-05T11:20:00Z",
  "visibility": "public",
  "license": "MIT",
  "repositoryUrl": "https://github.com/johndoe/medical-vit"
}
```

**Status Codes:**
- `201 Created` - Project created successfully
- `400 Bad Request` - Invalid project data

### Update Project

Update an existing project.

```http
PUT /api/v1/projects/{projectId}
Authorization: Bearer your-jwt-token
Content-Type: application/json

{
  "title": "Advanced Image Classification with Vision Transformer",
  "description": "An improved ViT-based image classification model for medical imaging",
  "fullDescription": "# Advanced Image Classification with Vision Transformer\n\nThis project implements...",
  "category": "Computer Vision",
  "tags": ["Vision Transformer", "Medical Imaging", "Image Classification", "Healthcare"],
  "visibility": "public",
  "license": "MIT",
  "repositoryUrl": "https://github.com/johndoe/medical-vit",
  "demoUrl": "https://demo.ouiai.com/johndoe/medical-vit"
}
```

**Response:**

```json
{
  "id": "e5f6g7h8-i9j0-k1l2-m3n4-o5p6q7r8s9t0",
  "title": "Advanced Image Classification with Vision Transformer",
  "description": "An improved ViT-based image classification model for medical imaging",
  "fullDescription": "# Advanced Image Classification with Vision Transformer\n\nThis project implements...",
  "category": "Computer Vision",
  "tags": ["Vision Transformer", "Medical Imaging", "Image Classification", "Healthcare"],
  "userId": "f7e3a9d1-b2c5-4d8e-a1f7-e9b2c5d8e1a7",
  "createdAt": "2023-04-05T11:20:00Z",
  "updatedAt": "2023-04-10T14:35:22Z",
  "visibility": "public",
  "license": "MIT",
  "repositoryUrl": "https://github.com/johndoe/medical-vit",
  "demoUrl": "https://demo.ouiai.com/johndoe/medical-vit"
}
```

**Status Codes:**
- `200 OK` - Project updated successfully
- `400 Bad Request` - Invalid project data
- `403 Forbidden` - User does not have permission to update the project
- `404 Not Found` - Project not found

### Delete Project

Delete an existing project.

```http
DELETE /api/v1/projects/{projectId}
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "message": "Project deleted successfully"
}
```

**Status Codes:**
- `200 OK` - Project deleted successfully
- `403 Forbidden` - User does not have permission to delete the project
- `404 Not Found` - Project not found

### Upload Project Thumbnail

Upload a thumbnail image for a project.

```http
POST /api/v1/projects/{projectId}/thumbnail
Authorization: Bearer your-jwt-token
Content-Type: multipart/form-data

file: [binary data]
```

**Response:**

```json
{
  "thumbnailUrl": "https://storage.ouiai.com/projects/thumbnails/medical-vit.jpg"
}
```

**Status Codes:**
- `200 OK` - Thumbnail uploaded successfully
- `400 Bad Request` - Invalid file format or size
- `403 Forbidden` - User does not have permission to update the project
- `404 Not Found` - Project not found

## Project Version Endpoints

### Create Project Version

Create a new version of an existing project.

```http
POST /api/v1/projects/{projectId}/versions
Authorization: Bearer your-jwt-token
Content-Type: application/json

{
  "version": "1.2.0",
  "description": "Added support for multi-class classification",
  "releaseNotes": "# Release Notes for v1.2.0\n\n- Added multi-class classification\n- Improved accuracy by 3%\n- Reduced model size by 10%"
}
```

**Response:**

```json
{
  "projectId": "e5f6g7h8-i9j0-k1l2-m3n4-o5p6q7r8s9t0",
  "version": "1.2.0",
  "description": "Added support for multi-class classification",
  "releaseNotes": "# Release Notes for v1.2.0\n\n- Added multi-class classification\n- Improved accuracy by 3%\n- Reduced model size by 10%",
  "createdAt": "2023-04-20T10:15:00Z",
  "userId": "f7e3a9d1-b2c5-4d8e-a1f7-e9b2c5d8e1a7",
  "username": "johndoe"
}
```

**Status Codes:**
- `201 Created` - Version created successfully
- `400 Bad Request` - Invalid version data
- `403 Forbidden` - User does not have permission to update the project
- `404 Not Found` - Project not found
- `409 Conflict` - Version already exists

### Get Project Versions

Get all versions of a project.

```http
GET /api/v1/projects/{projectId}/versions
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "items": [
    {
      "version": "1.0.0",
      "description": "Initial release",
      "createdAt": "2023-03-10T15:30:00Z",
      "userId": "f7e3a9d1-b2c5-4d8e-a1f7-e9b2c5d8e1a7",
      "username": "johndoe"
    },
    {
      "version": "1.1.0",
      "description": "Improved accuracy by 5%",
      "createdAt": "2023-03-15T09:45:30Z",
      "userId": "f7e3a9d1-b2c5-4d8e-a1f7-e9b2c5d8e1a7",
      "username": "johndoe"
    },
    {
      "version": "1.2.0",
      "description": "Added support for multi-class classification",
      "createdAt": "2023-04-20T10:15:00Z",
      "userId": "f7e3a9d1-b2c5-4d8e-a1f7-e9b2c5d8e1a7",
      "username": "johndoe"
    }
  ]
}
```

**Status Codes:**
- `200 OK` - Versions retrieved successfully
- `404 Not Found` - Project not found

## Project Collaboration Endpoints

### Add Collaborator

Add a collaborator to a project.

```http
POST /api/v1/projects/{projectId}/collaborators
Authorization: Bearer your-jwt-token
Content-Type: application/json

{
  "userId": "b3c5d7e9-f1a3-b5c7-d9e1-f3a5b7c9d1e3",
  "role": "contributor"
}
```

**Response:**

```json
{
  "projectId": "e5f6g7h8-i9j0-k1l2-m3n4-o5p6q7r8s9t0",
  "userId": "b3c5d7e9-f1a3-b5c7-d9e1-f3a5b7c9d1e3",
  "username": "janedoe",
  "avatarUrl": "https://storage.ouiai.com/avatars/janedoe.jpg",
  "role": "contributor",
  "addedAt": "2023-04-25T16:20:00Z"
}
```

**Status Codes:**
- `201 Created` - Collaborator added successfully
- `400 Bad Request` - Invalid data
- `403 Forbidden` - User does not have permission to manage collaborators
- `404 Not Found` - Project or user not found
- `409 Conflict` - User is already a collaborator

### Remove Collaborator

Remove a collaborator from a project.

```http
DELETE /api/v1/projects/{projectId}/collaborators/{userId}
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "message": "Collaborator removed successfully"
}
```

**Status Codes:**
- `200 OK` - Collaborator removed successfully
- `403 Forbidden` - User does not have permission to manage collaborators
- `404 Not Found` - Project, user, or collaboration not found

### Get Project Collaborators

Get all collaborators for a project.

```http
GET /api/v1/projects/{projectId}/collaborators
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "items": [
    {
      "userId": "f7e3a9d1-b2c5-4d8e-a1f7-e9b2c5d8e1a7",
      "username": "johndoe",
      "avatarUrl": "https://storage.ouiai.com/avatars/johndoe.jpg",
      "role": "owner",
      "addedAt": "2023-03-10T15:30:00Z"
    },
    {
      "userId": "b3c5d7e9-f1a3-b5c7-d9e1-f3a5b7c9d1e3",
      "username": "janedoe",
      "avatarUrl": "https://storage.ouiai.com/avatars/janedoe.jpg",
      "role": "contributor",
      "addedAt": "2023-04-25T16:20:00Z"
    }
  ]
}
```

**Status Codes:**
- `200 OK` - Collaborators retrieved successfully
- `404 Not Found` - Project not found

## Project Interaction Endpoints

### Like a Project

Like a project.

```http
POST /api/v1/projects/{projectId}/like
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "likesCount": 43,
  "isLiked": true
}
```

**Status Codes:**
- `200 OK` - Project liked successfully
- `404 Not Found` - Project not found

### Unlike a Project

Remove a like from a project.

```http
DELETE /api/v1/projects/{projectId}/like
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "likesCount": 42,
  "isLiked": false
}
```

**Status Codes:**
- `200 OK` - Project unliked successfully
- `404 Not Found` - Project not found

### Bookmark a Project

Bookmark a project.

```http
POST /api/v1/projects/{projectId}/bookmark
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "isBookmarked": true
}
```

**Status Codes:**
- `200 OK` - Project bookmarked successfully
- `404 Not Found` - Project not found

### Remove Bookmark

Remove a bookmark from a project.

```http
DELETE /api/v1/projects/{projectId}/bookmark
Authorization: Bearer your-jwt-token
```

**Response:**

```json
{
  "isBookmarked": false
}
```

**Status Codes:**
- `200 OK` - Bookmark removed successfully
- `404 Not Found` - Project not found

## Error Responses

### Invalid Project Data

```json
{
  "status": 400,
  "message": "Validation failed.",
  "errors": {
    "Title": ["The Title field is required."],
    "Category": ["The Category field is required."]
  },
  "traceId": "7f8d3a2e-1c5b-4d9a-8f7e-3a2c1d9b5a7e"
}
```

### Project Not Found

```json
{
  "status": 404,
  "message": "Project with ID 'e5f6g7h8-i9j0-k1l2-m3n4-o5p6q7r8s9t0' not found.",
  "errors": null,
  "traceId": "7f8d3a2e-1c5b-4d9a-8f7e-3a2c1d9b5a7e"
}
```

### Permission Denied

```json
{
  "status": 403,
  "message": "You do not have permission to update this project.",
  "errors": null,
  "traceId": "7f8d3a2e-1c5b-4d9a-8f7e-3a2c1d9b5a7e"
}
```

## Data Models

### ProjectCreateDto

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| title | string | Yes | Project title |
| description | string | Yes | Short project description |
| fullDescription | string | No | Full project description (Markdown) |
| category | string | Yes | Project category |
| tags | array of strings | No | Project tags |
| visibility | string | Yes | Project visibility (public, private) |
| license | string | No | Project license |
| repositoryUrl | string | No | URL to project repository |
| demoUrl | string | No | URL to project demo |

### ProjectUpdateDto

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| title | string | No | Project title |
| description | string | No | Short project description |
| fullDescription | string | No | Full project description (Markdown) |
| category | string | No | Project category |
| tags | array of strings | No | Project tags |
| visibility | string | No | Project visibility (public, private) |
| license | string | No | Project license |
| repositoryUrl | string | No | URL to project repository |
| demoUrl | string | No | URL to project demo |

### ProjectVersionDto

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| version | string | Yes | Version number (semantic versioning) |
| description | string | Yes | Version description |
| releaseNotes | string | No | Release notes (Markdown) |

### CollaboratorDto

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| userId | string | Yes | User ID of collaborator |
| role | string | Yes | Collaborator role (contributor, admin) |
