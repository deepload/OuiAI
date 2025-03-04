# OuiAI Troubleshooting Guide

This document provides solutions for common issues you might encounter while developing, deploying, or using the OuiAI platform.

## Table of Contents

- [Development Environment Issues](#development-environment-issues)
- [Backend Issues](#backend-issues)
- [Frontend Issues](#frontend-issues)
- [Database Issues](#database-issues)
- [Authentication Issues](#authentication-issues)
- [API Issues](#api-issues)
- [Performance Issues](#performance-issues)
- [Deployment Issues](#deployment-issues)
- [Common Error Messages](#common-error-messages)
- [Getting Help](#getting-help)

## Development Environment Issues

### .NET SDK Version Mismatch

**Problem**: Error message about SDK version not matching the project's target framework.

**Solution**:
1. Check the required .NET SDK version in the project files
2. Install the correct .NET SDK version from the [.NET Downloads page](https://dotnet.microsoft.com/download)
3. Verify installation with `dotnet --info`

### Node.js Version Mismatch

**Problem**: Error messages about unsupported Node.js features or package incompatibilities.

**Solution**:
1. Check the required Node.js version in `package.json`
2. Install the correct Node.js version using [NVM (Node Version Manager)](https://github.com/nvm-sh/nvm)
3. Verify installation with `node --version`

### Docker Issues

**Problem**: Docker containers fail to start or communicate with each other.

**Solution**:
1. Verify Docker Desktop is running
2. Check container logs: `docker logs <container_name>`
3. Ensure ports are not in use by other applications
4. Restart Docker Desktop
5. Rebuild containers: `docker-compose build`
6. Start containers with fresh state: `docker-compose down -v && docker-compose up -d`

### Visual Studio Issues

**Problem**: Visual Studio crashes, slow performance, or build errors.

**Solution**:
1. Restart Visual Studio
2. Clear Visual Studio cache: `%LOCALAPPDATA%\Microsoft\VisualStudio\<version>\ComponentModelCache`
3. Run Visual Studio as administrator
4. Repair Visual Studio installation using the Visual Studio Installer
5. Update to the latest Visual Studio version

## Backend Issues

### Microservice Communication Failures

**Problem**: Microservices unable to communicate with each other.

**Solution**:
1. Verify all services are running: `docker ps` or check service health endpoints
2. Check network configurations in `docker-compose.yml`
3. Verify service URLs in configuration files
4. Check logs for connection errors
5. Test direct service-to-service communication using tools like curl

```powershell
# Example: Testing Gateway to Identity microservice communication
$headers = @{
    "Content-Type" = "application/json"
}
Invoke-RestMethod -Uri "http://localhost:5001/api/v1/identity/health" -Method Get -Headers $headers
```

### Entity Framework Migration Issues

**Problem**: Database update fails or Entity Framework migration errors.

**Solution**:
1. Verify connection string in `appsettings.json`
2. Ensure SQL Server is running
3. Check migration files for errors
4. Try removing the latest migration: `dotnet ef migrations remove`
5. Add a new migration: `dotnet ef migrations add <MigrationName>`
6. Apply migration: `dotnet ef database update`

### Dependency Injection Issues

**Problem**: "Unable to resolve service" errors or null reference exceptions for injected services.

**Solution**:
1. Verify service registration in `Startup.cs`
2. Check for circular dependencies
3. Verify service lifetimes (Singleton, Scoped, Transient)
4. Check constructor parameters match registered service types

```csharp
// Example of correct service registration
public void ConfigureServices(IServiceCollection services)
{
    // Register services
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IAuthService, AuthService>();
}
```

## Frontend Issues

### npm Package Issues

**Problem**: npm install fails or package conflicts.

**Solution**:
1. Delete `node_modules` folder and `package-lock.json`
2. Clear npm cache: `npm cache clean --force`
3. Reinstall packages: `npm install`
4. Check for outdated packages: `npm outdated`
5. Update packages: `npm update`

### React Component Rendering Issues

**Problem**: Components not rendering as expected or rendering errors.

**Solution**:
1. Check React DevTools for component state and props
2. Verify component lifecycle methods or hooks are used correctly
3. Check for missing keys in lists
4. Look for conditional rendering errors
5. Check for null/undefined values causing rendering issues

```javascript
// Example of safe conditional rendering
const UserProfile = ({ user }) => {
  // Early return if user is not available
  if (!user) {
    return <div>Loading user profile...</div>;
  }

  return (
    <div>
      <h2>{user.name}</h2>
      <p>{user.email}</p>
      {user.bio && <p>{user.bio}</p>}
      {user.projects && user.projects.length > 0 && (
        <ul>
          {user.projects.map(project => (
            <li key={project.id}>{project.name}</li>
          ))}
        </ul>
      )}
    </div>
  );
};
```

### State Management Issues

**Problem**: Application state not updating or inconsistent state between components.

**Solution**:
1. Use Redux DevTools to inspect state changes
2. Verify actions are dispatched correctly
3. Check reducer logic for state updates
4. Ensure components are connected to the store correctly
5. Verify selectors are returning expected data

### API Integration Issues

**Problem**: Frontend not connecting to backend APIs or receiving errors.

**Solution**:
1. Verify API endpoint URLs
2. Check network requests in browser DevTools
3. Ensure authentication tokens are included in requests
4. Verify CORS configuration on the backend
5. Test API endpoints with Postman

## Database Issues

### Connection Issues

**Problem**: Unable to connect to database.

**Solution**:
1. Verify database server is running
2. Check connection string in `appsettings.json`
3. Ensure firewall allows connections to database port
4. Verify database credentials
5. Check database server logs for errors

```json
// Example connection string in appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=OuiAI;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
  }
}
```

### Query Performance Issues

**Problem**: Slow database queries.

**Solution**:
1. Analyze query execution plan
2. Add appropriate indexes to tables
3. Optimize EF Core queries (use Include, projections, etc.)
4. Implement caching for frequently accessed data
5. Consider database tuning (SQL Server configuration)

### Schema Migration Issues

**Problem**: Database schema changes causing errors.

**Solution**:
1. Backup database before migrations
2. Test migrations in a development environment first
3. Use a clear naming convention for migrations
4. Include both Up() and Down() methods in migrations
5. Verify database schema after migration

## Authentication Issues

### JWT Token Issues

**Problem**: Invalid tokens or token validation failures.

**Solution**:
1. Check token expiration time
2. Verify JWT secret key is consistent across services
3. Ensure token validation parameters match token generation parameters
4. Check for clock skew between servers
5. Inspect token using [jwt.io](https://jwt.io/) to verify claims

```csharp
// Example of token validation configuration
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
            ValidateIssuer = true,
            ValidIssuer = Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // Remove clock skew to make expiration exact
        };
    });
```

### Login Failures

**Problem**: Users unable to login.

**Solution**:
1. Verify user credentials in the database
2. Check password hashing configuration
3. Enable more detailed error logging temporarily
4. Look for CORS issues if login API is on a different domain
5. Verify authentication service is running

### Authorization Issues

**Problem**: Users receiving 403 Forbidden errors.

**Solution**:
1. Verify user roles and claims
2. Check authorization policies
3. Review role requirements on controllers and actions
4. Ensure user has necessary permissions in the database
5. Verify token contains required claims

## API Issues

### 404 Not Found Errors

**Problem**: API endpoints returning 404 errors.

**Solution**:
1. Verify route configuration
2. Check URL path and HTTP method
3. Ensure controller and action exist
4. Check for typos in the URL
5. Verify API version in the URL

### 500 Internal Server Errors

**Problem**: API endpoints returning 500 errors.

**Solution**:
1. Check server logs for detailed error information
2. Look for exceptions in application code
3. Verify database connections
4. Check external service dependencies
5. Enable developer exception page in development

```csharp
// In Configure method of Startup.cs
if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}
```

### Rate Limiting Issues

**Problem**: Too many requests error (429).

**Solution**:
1. Check rate limiting configuration
2. Implement client-side retry with exponential backoff
3. Optimize client code to reduce API calls
4. Consider increasing rate limits for specific clients
5. Implement caching to reduce API calls

## Performance Issues

### Slow API Response Times

**Problem**: API endpoints taking too long to respond.

**Solution**:
1. Check database query performance
2. Use profiling tools to identify bottlenecks
3. Implement caching for frequently accessed data
4. Optimize data access patterns
5. Consider asynchronous processing for long-running operations

### Memory Leaks

**Problem**: Application memory usage grows over time.

**Solution**:
1. Use memory profiling tools
2. Check for unmanaged resources not being disposed
3. Verify IDisposable implementations
4. Look for large objects stored in memory
5. Check for event handler leaks

### High CPU Usage

**Problem**: Application consuming excessive CPU resources.

**Solution**:
1. Use CPU profiling tools
2. Look for infinite loops or recursive calls
3. Optimize expensive operations
4. Consider parallel processing for CPU-intensive tasks
5. Implement caching to reduce computation

## Deployment Issues

### Docker Deployment Issues

**Problem**: Docker containers failing in production.

**Solution**:
1. Verify Docker image builds successfully
2. Check container logs
3. Ensure environment variables are set correctly
4. Verify volume mounts and permissions
5. Check network configuration

```bash
# Commands to troubleshoot Docker issues
docker ps -a  # List all containers including stopped ones
docker logs <container_id>  # View container logs
docker exec -it <container_id> sh  # Open shell in container
docker-compose config  # Validate docker-compose.yml
```

### Azure Deployment Issues

**Problem**: Azure services not working as expected.

**Solution**:
1. Check Azure portal for service health
2. Review Azure resource metrics and logs
3. Verify connection strings and configuration
4. Check network security groups
5. Review Azure App Service logs

### Kubernetes Deployment Issues

**Problem**: Kubernetes pods not starting or crashing.

**Solution**:
1. Check pod status: `kubectl get pods`
2. View pod logs: `kubectl logs <pod_name>`
3. Describe pod for events: `kubectl describe pod <pod_name>`
4. Verify resource limits and requests
5. Check image pull policies and secrets

## Common Error Messages

### "System.InvalidOperationException: No service for type 'X' has been registered."

**Cause**: Service not registered in DI container.

**Solution**: Register the service in `Startup.cs`:

```csharp
services.AddScoped<IYourService, YourService>();
```

### "CORS policy execution failed."

**Cause**: Cross-Origin Resource Sharing policy blocking requests.

**Solution**: Configure CORS properly:

```csharp
services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("https://yourfrontend.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// In Configure method
app.UseCors("AllowSpecificOrigin");
```

### "The Entity Framework Core database operation failed."

**Cause**: Database operation error.

**Solution**:
1. Check connection string
2. Verify migration status: `dotnet ef migrations list`
3. Update database: `dotnet ef database update`
4. Check for model validation errors

### "JWT token validation failed."

**Cause**: Issues with JWT token.

**Solution**:
1. Check token expiration
2. Verify signing key
3. Ensure token claims match expected values

## Getting Help

If you encounter issues not covered in this guide:

1. **Check Logs**: Application logs are the first place to look for error details.
2. **Search Documentation**: Check if the issue is covered in other documentation.
3. **Team Chat**: Ask in the development team's chat channel.
4. **File an Issue**: Create a detailed issue in the project's issue tracker.
5. **Stack Overflow**: For general technical questions, consider searching or asking on Stack Overflow.

### Support Contacts

- **Development Team**: dev-team@ouiai.com
- **DevOps Team**: devops@ouiai.com
- **Security Team**: security@ouiai.com
