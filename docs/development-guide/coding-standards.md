# OuiAI Coding Standards

This document outlines the coding standards and best practices for the OuiAI platform. Following these standards ensures code consistency, maintainability, and quality across the codebase.

## Table of Contents

- [General Guidelines](#general-guidelines)
- [C# Coding Standards](#c-coding-standards)
- [JavaScript/TypeScript Coding Standards](#javascripttypescript-coding-standards)
- [SQL Coding Standards](#sql-coding-standards)
- [API Design Guidelines](#api-design-guidelines)
- [Code Reviews](#code-reviews)
- [Documentation](#documentation)
- [Logging](#logging)
- [Error Handling](#error-handling)
- [Security Practices](#security-practices)

## General Guidelines

### Principles

- **Readability**: Code should be easy to read and understand
- **Maintainability**: Code should be easy to modify and extend
- **Simplicity**: Prefer simple solutions over complex ones
- **Consistency**: Follow established patterns and practices
- **Performance**: Write efficient code, optimize when necessary
- **Testability**: Design code to be testable

### DRY Principle

Don't Repeat Yourself (DRY): Avoid code duplication by abstracting common functionality.

### SOLID Principles

- **Single Responsibility**: Classes should have only one reason to change
- **Open/Closed**: Open for extension, closed for modification
- **Liskov Substitution**: Subtypes must be substitutable for their base types
- **Interface Segregation**: Many specific interfaces are better than one general interface
- **Dependency Inversion**: Depend on abstractions, not concretions

### Code Organization

- Organize code logically in directories/namespaces
- Keep files focused on a single responsibility
- Break large files into smaller, more manageable components
- Use consistent file naming conventions

## C# Coding Standards

### Naming Conventions

| Item | Convention | Example |
|------|------------|---------|
| Namespace | PascalCase | `OuiAI.Microservices.Identity` |
| Class | PascalCase | `UserService` |
| Interface | IPascalCase | `IUserService` |
| Method | PascalCase | `GetUserProfile` |
| Property | PascalCase | `FirstName` |
| Field (private) | _camelCase | `_userRepository` |
| Field (protected) | _camelCase | `_connectionString` |
| Constant | PascalCase | `MaxRetryCount` |
| Parameter | camelCase | `userId` |
| Local variable | camelCase | `userProfile` |
| Enum | PascalCase | `UserStatus` |
| Enum value | PascalCase | `Active` |

### File Organization

- One class per file (with exceptions for small related classes)
- File name should match the primary class name
- Organize files in directories according to their namespace

### Code Structure (Classes)

```csharp
namespace OuiAI.Microservices.Identity.Services
{
    /// <summary>
    /// Provides user management services.
    /// </summary>
    public class UserService : IUserService
    {
        // 1. Private fields
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        // 2. Constructors
        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // 3. Public properties
        public int MaxRetryCount { get; set; } = 3;

        // 4. Public methods
        public async Task<UserProfile> GetUserProfileAsync(string userId)
        {
            _logger.LogInformation("Getting user profile for user {UserId}", userId);
            
            // Implementation
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                throw new UserNotFoundException(userId);
            }
            
            return MapToUserProfile(user);
        }

        // 5. Private methods
        private UserProfile MapToUserProfile(User user)
        {
            // Mapping logic
        }
    }
}
```

### C# Best Practices

- Use `var` only when the type is obvious from the context
- Prefer async/await over traditional callbacks
- Use nullable reference types
- Use pattern matching for type checking
- Use expression-bodied members for simple methods
- Use tuples for simple return types
- Use records for DTOs
- Use init-only properties for immutable objects
- Use string interpolation for string formatting
- Use nameof() for exception messages and logging

### Exception Handling

- Use specific exception types
- Don't catch exceptions unless you can handle them
- Use try/catch blocks only when necessary
- Log exceptions with context
- Rethrow exceptions properly using `throw;`

```csharp
try
{
    // Code that might throw
}
catch (SpecificException ex)
{
    _logger.LogError(ex, "Error while processing {UserId}", userId);
    throw new ApplicationException("Failed to process user", ex);
}
```

### LINQ Usage

- Use method syntax for complex queries
- Use query syntax for simpler queries
- Avoid multiple enumerations of the same sequence
- Use appropriate methods (e.g., FirstOrDefault instead of First when appropriate)

### Asynchronous Programming

- Use async/await for I/O-bound operations
- Suffix asynchronous methods with "Async"
- Use TaskCompletionSource for custom asynchronous operations
- Use CancellationToken for cancelable operations
- Avoid async void methods (except for event handlers)

## JavaScript/TypeScript Coding Standards

### Naming Conventions

| Item | Convention | Example |
|------|------------|---------|
| Component | PascalCase | `UserProfile` |
| Function | camelCase | `getUserProfile` |
| Variable | camelCase | `userData` |
| Constant | UPPER_SNAKE_CASE | `MAX_RETRY_COUNT` |
| File | kebab-case | `user-profile.tsx` |
| Interface | PascalCase | `UserData` |
| Type | PascalCase | `UserStatus` |
| Enum | PascalCase | `UserRole` |

### File Organization

- One component per file (with exceptions for small related components)
- File name should match the component name (in kebab-case)
- Group related files in directories

### React Component Structure

```tsx
// Import statements
import React, { useState, useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';

// Interface definitions
interface UserProfileProps {
  userId: string;
  editable?: boolean;
}

// Component definition
const UserProfile: React.FC<UserProfileProps> = ({ userId, editable = false }) => {
  // Hooks
  const dispatch = useDispatch();
  const user = useSelector((state) => state.users.currentUser);
  const [isLoading, setIsLoading] = useState(false);

  // Effects
  useEffect(() => {
    const fetchUser = async () => {
      setIsLoading(true);
      try {
        await dispatch(fetchUserProfile(userId));
      } catch (error) {
        console.error('Error fetching user profile:', error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchUser();
  }, [userId, dispatch]);

  // Event handlers
  const handleUpdateProfile = () => {
    // Update profile logic
  };

  // Conditional rendering
  if (isLoading) {
    return <Spinner />;
  }

  if (!user) {
    return <div>User not found</div>;
  }

  // JSX
  return (
    <div className="user-profile">
      <h2>{user.fullName}</h2>
      {/* More JSX */}
      {editable && (
        <button onClick={handleUpdateProfile}>Edit Profile</button>
      )}
    </div>
  );
};

// Export statement
export default UserProfile;
```

### JavaScript/TypeScript Best Practices

- Use TypeScript for type safety
- Use ES6+ features (destructuring, spread operator, etc.)
- Use functional components with hooks
- Use async/await for asynchronous operations
- Use optional chaining and nullish coalescing
- Avoid any type when possible
- Use enums for constants with logical grouping
- Use interfaces for object shapes
- Use type aliases for union types

### Component Best Practices

- Keep components focused on a single responsibility
- Extract complex logic into custom hooks
- Use React.memo for performance optimization
- Use React.lazy for code splitting
- Avoid inline styles (use CSS modules or styled-components)
- Use propTypes or TypeScript for prop validation
- Use default props for optional props

### State Management

- Use Redux for complex global state
- Use Context API for simpler shared state
- Use useState for component-local state
- Use useReducer for complex component state
- Normalize state in Redux store
- Use Redux Toolkit for Redux best practices

## SQL Coding Standards

### Naming Conventions

| Item | Convention | Example |
|------|------------|---------|
| Table | PascalCase, Plural | `Users` |
| Column | PascalCase | `FirstName` |
| Primary Key | PascalCase, ID suffix | `UserId` |
| Foreign Key | PascalCase, Referenced table + ID | `ProjectId` |
| Stored Procedure | PascalCase, Verb + Noun | `GetUserProfile` |
| Index | IX_TableName_Column1_Column2 | `IX_Users_Email` |
| Constraint | CK_TableName_Constraint | `CK_Users_Email` |

### Formatting

- Use uppercase for SQL keywords
- Use proper indentation for readability
- Break long statements into multiple lines
- Use aliases for table names
- Align columns in SELECT statements

```sql
SELECT 
    u.UserId,
    u.FirstName,
    u.LastName,
    u.Email,
    p.Name AS ProjectName
FROM 
    Users u
    INNER JOIN UserProjects up ON u.UserId = up.UserId
    INNER JOIN Projects p ON up.ProjectId = p.ProjectId
WHERE 
    u.Status = 'Active'
    AND p.IsDeleted = 0
ORDER BY 
    u.LastName,
    u.FirstName;
```

### SQL Best Practices

- Use parameters to prevent SQL injection
- Avoid SELECT * (specify columns explicitly)
- Use appropriate indexing for performance
- Use transactions for multi-statement operations
- Use stored procedures for complex operations
- Use appropriate data types
- Include appropriate constraints
- Use NOLOCK hints only when appropriate

## API Design Guidelines

### RESTful API Design

- Use nouns for resource names (e.g., `/users` not `/getUsers`)
- Use plural nouns for collections (e.g., `/users`, not `/user`)
- Use HTTP methods appropriately:
  - GET: Retrieve resources
  - POST: Create resources
  - PUT: Update resources (full update)
  - PATCH: Partial update resources
  - DELETE: Remove resources
- Use sub-resources for relationships (e.g., `/users/{id}/projects`)
- Use query parameters for filtering, sorting, and pagination
- Return appropriate HTTP status codes

### Versioning

- Include version in URL path (e.g., `/api/v1/users`)
- Maintain backward compatibility within a version
- Document breaking changes between versions

### Request/Response Format

- Use JSON for request/response bodies
- Use camelCase for JSON property names
- Include pagination metadata for collection responses
- Use consistent error response format
- Include request ID for troubleshooting

### API Security

- Use HTTPS for all API endpoints
- Implement proper authentication and authorization
- Validate input data
- Implement rate limiting
- Use appropriate CORS settings

## Code Reviews

### Code Review Process

1. **Preparation**: Ensure code is ready for review (tested, documented, etc.)
2. **Submission**: Create a pull request with a clear description
3. **Review**: Reviewers examine code for issues
4. **Discussion**: Address comments and questions
5. **Updates**: Make necessary changes based on feedback
6. **Approval**: Obtain required approvals
7. **Merge**: Merge the code into the target branch

### Code Review Checklist

- Code follows coding standards
- Tests are included and pass
- Documentation is updated
- No security vulnerabilities
- No performance issues
- Error handling is appropriate
- No unnecessary code or comments
- No unused dependencies

### Giving Feedback

- Be specific and constructive
- Explain why, not just what
- Focus on the code, not the person
- Provide examples when possible
- Note positive aspects, not just issues
- Prioritize feedback (critical vs. nice-to-have)

## Documentation

### Code Documentation

- Use XML comments for C# code
- Use JSDoc comments for JavaScript/TypeScript code
- Document public APIs thoroughly
- Include parameter and return value descriptions
- Document exceptions that may be thrown
- Update documentation when code changes

```csharp
/// <summary>
/// Gets a user's profile by their ID.
/// </summary>
/// <param name="userId">The unique identifier of the user.</param>
/// <returns>The user's profile information.</returns>
/// <exception cref="UserNotFoundException">Thrown when the user doesn't exist.</exception>
public async Task<UserProfile> GetUserProfileAsync(string userId)
{
    // Implementation
}
```

```typescript
/**
 * Fetches a user's profile from the API.
 * @param userId - The unique identifier of the user
 * @returns A promise that resolves to the user profile data
 * @throws Will throw an error if the API request fails
 */
async function fetchUserProfile(userId: string): Promise<UserProfile> {
    // Implementation
}
```

### API Documentation

- Use Swagger/OpenAPI for API documentation
- Document all endpoints, parameters, and responses
- Include examples
- Document error responses
- Keep documentation up-to-date with code changes

## Logging

### Logging Guidelines

- Log meaningful events at appropriate levels:
  - **Trace**: Detailed debugging information
  - **Debug**: General debugging information
  - **Information**: General information about application flow
  - **Warning**: Non-critical issues that might need attention
  - **Error**: Errors that impact functionality but don't crash the application
  - **Critical**: Critical errors that might crash the application
- Include context in log messages (e.g., user ID, request ID)
- Use structured logging when possible
- Don't log sensitive information
- Log at entry and exit points of significant operations

```csharp
// Good logging
_logger.LogInformation("User {UserId} updated profile", user.Id);

// Bad logging
_logger.LogInformation("User updated profile");
```

## Error Handling

### Error Handling Guidelines

- Handle specific exceptions, not just generic ones
- Log exceptions with context
- Provide meaningful error messages to users
- Don't expose sensitive information in error messages
- Return appropriate HTTP status codes for API errors
- Use a consistent error response format

### Error Response Format

```json
{
  "status": 400,
  "message": "Invalid user data",
  "errors": {
    "email": ["Email address is invalid"],
    "password": ["Password must be at least 8 characters"]
  },
  "traceId": "7f8d3a2e-1c5b-4d9a-8f7e-3a2c1d9b5a7e"
}
```

## Security Practices

### Security Guidelines

- **Authentication**: Implement proper authentication mechanisms
- **Authorization**: Ensure users can only access what they're allowed to
- **Data Validation**: Validate all input data
- **Encryption**: Encrypt sensitive data
- **XSS Prevention**: Escape user-generated content
- **CSRF Protection**: Implement CSRF tokens
- **CORS**: Configure appropriate CORS settings
- **Rate Limiting**: Implement rate limiting to prevent abuse
- **Dependency Management**: Keep dependencies updated
- **Security Headers**: Set appropriate security headers

### Sensitive Data

- Don't store sensitive data in code or configuration files
- Use environment variables or secure stores for secrets
- Hash passwords with strong algorithms (bcrypt, Argon2)
- Encrypt sensitive data before storing
