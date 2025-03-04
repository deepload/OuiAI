# OuiAI API Documentation

This directory contains documentation for the OuiAI API, which serves as the interface between the frontend applications and the backend microservices.

## Overview

The OuiAI API follows REST principles and uses JSON for data exchange. All endpoints are accessible through the API Gateway, which routes requests to the appropriate microservices.

## API Base URLs

- **Development**: `https://localhost:5001`
- **Staging**: `https://staging-api.ouiai.com`
- **Production**: `https://api.ouiai.com`

## Authentication

Most API endpoints require authentication. The OuiAI API uses JWT (JSON Web Tokens) for authentication.

### Obtaining a Token

To obtain a JWT token, send a POST request to the `/api/auth/login` endpoint with your credentials.

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "your_password"
}
```

### Using the Token

Include the JWT token in the Authorization header of your requests:

```http
GET /api/users/profile
Authorization: Bearer your_jwt_token
```

## Rate Limiting

The API implements rate limiting to prevent abuse. Limits are as follows:

- **Authenticated requests**: 100 requests per minute
- **Unauthenticated requests**: 30 requests per minute

When a rate limit is exceeded, the API returns a 429 Too Many Requests response.

## Error Handling

API errors follow a consistent format:

```json
{
  "status": 400,
  "message": "Error message",
  "errors": {
    "field_name": ["Error description"]
  },
  "traceId": "7f8d3a2e-1c5b-4d9a-8f7e-3a2c1d9b5a7e"
}
```

## Common Status Codes

| Status Code | Description |
|-------------|-------------|
| 200 | OK - The request was successful |
| 201 | Created - A new resource was created |
| 204 | No Content - The request was successful but no content is returned |
| 400 | Bad Request - The request contains invalid parameters |
| 401 | Unauthorized - Authentication is required |
| 403 | Forbidden - The authenticated user does not have sufficient permissions |
| 404 | Not Found - The requested resource does not exist |
| 429 | Too Many Requests - Rate limit exceeded |
| 500 | Internal Server Error - An error occurred on the server |

## API Services

The OuiAI API is organized into the following services:

1. [Identity API](./identity-api.md) - User authentication and profile management
2. [Projects API](./projects-api.md) - AI project management
3. [Social API](./social-api.md) - Social interactions and activity feed
4. [Notifications API](./notifications-api.md) - User notifications
5. [Search API](./search-api.md) - Search functionality

## API Versioning

The OuiAI API uses URL path versioning. The current version is v1:

```
https://api.ouiai.com/api/v1/users/profile
```

## Cross-Origin Resource Sharing (CORS)

The API supports CORS for the following origins:
- `https://ouiai.com`
- `https://www.ouiai.com`
- `https://app.ouiai.com`
- `https://staging.ouiai.com`
- `http://localhost:3000` (development only)

## Pagination

Endpoints that return collections support pagination with the following query parameters:

- `page` - The page number (1-based)
- `pageSize` - The number of items per page (default: 20, max: 100)

Paginated responses include pagination metadata:

```json
{
  "items": [...],
  "pagination": {
    "totalItems": 245,
    "totalPages": 13,
    "currentPage": 2,
    "pageSize": 20
  }
}
```

## Filtering and Sorting

Collection endpoints support filtering and sorting:

- Filtering: `?filter.fieldName=value`
- Sorting: `?sort=fieldName` or `?sort=-fieldName` (descending)

Example:
```
GET /api/projects?filter.category=AI&sort=-createdAt
```

## Interactive Documentation

Interactive API documentation is available using Swagger UI:

- Development: `https://localhost:5001/swagger`
- Staging: `https://staging-api.ouiai.com/swagger`
- Production: `https://api.ouiai.com/swagger` (only in staging mode)

## Postman Collection

A Postman collection is available in the [postman](./postman) directory. Import this collection to test the API using Postman.

## API Client Libraries

OuiAI provides client libraries for easy integration:

- [JavaScript/TypeScript](https://github.com/ouiai/api-client-js)
- [C# .NET](https://github.com/ouiai/api-client-dotnet)

## API Change Log

See the [CHANGELOG.md](./CHANGELOG.md) file for a list of API changes.

## Support

For API support, please contact our developer support team at api-support@ouiai.com.
