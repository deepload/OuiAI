# OuiAI Testing Guide

This document provides comprehensive testing procedures for the OuiAI platform to ensure all components function correctly.

## Table of Contents
- [Testing Strategy](#testing-strategy)
- [Test Environment Setup](#test-environment-setup)
- [Backend Testing](#backend-testing)
- [Frontend Testing](#frontend-testing)
- [API Testing](#api-testing)
- [Integration Testing](#integration-testing)
- [End-to-End Testing](#end-to-end-testing)
- [Performance Testing](#performance-testing)
- [Security Testing](#security-testing)
- [Test Cases](#test-cases)

## Testing Strategy

OuiAI follows a comprehensive testing strategy that includes:

1. **Unit Testing**: Testing individual components in isolation
2. **Integration Testing**: Testing interactions between components
3. **API Testing**: Testing REST API endpoints
4. **UI Testing**: Testing user interfaces for both web and mobile
5. **End-to-End Testing**: Testing complete user flows
6. **Performance Testing**: Testing system performance under load
7. **Security Testing**: Testing for vulnerabilities and security issues

## Test Environment Setup

### Local Testing Environment

1. **Prerequisites**:
   - All the software mentioned in the INSTALLATION.md guide
   - Test databases separate from development databases

2. **Setup Test Databases**:
   ```sql
   CREATE DATABASE OuiAI_Test_Identity;
   CREATE DATABASE OuiAI_Test_Projects;
   CREATE DATABASE OuiAI_Test_Social;
   CREATE DATABASE OuiAI_Test_Notifications;
   CREATE DATABASE OuiAI_Test_Search;
   GO
   ```

3. **Configure Test Settings**:
   - Create `appsettings.Test.json` files for each microservice with test database connection strings
   - Create `.env.test` files for frontend applications

## Backend Testing

### Running Backend Unit Tests

```powershell
# Navigate to test project directory
cd C:\Projects\OuiAI\tests\Backend.Tests

# Run all tests
dotnet test

# Run tests for a specific microservice
dotnet test --filter "Category=IdentityTests"
dotnet test --filter "Category=ProjectsTests"
dotnet test --filter "Category=SocialTests"
```

### Backend Test Coverage

To generate test coverage reports:

```powershell
# Install ReportGenerator
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate coverage report
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Generate HTML report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
```

## Frontend Testing

### Running Web Frontend Tests

```powershell
# Navigate to web frontend directory
cd C:\Projects\OuiAI\src\Frontend\OuiAI.Web\ouiai-web

# Run tests
npm test

# Run tests with coverage
npm test -- --coverage

# Run specific test file
npm test -- -t "AuthContext"
```

### Running Mobile Frontend Tests

```powershell
# Navigate to mobile frontend directory
cd C:\Projects\OuiAI\src\Frontend\OuiAI.Mobile\OuiAI

# Run tests
npm test

# Run tests with coverage
npm test -- --coverage
```

## API Testing

### Using Swagger UI

1. Start the respective microservice
2. Navigate to https://localhost:<port>/swagger
3. Use the Swagger UI to test API endpoints

### Using Postman

1. Import the Postman collection from `docs/api/OuiAI.postman_collection.json`
2. Set up environment variables for local testing
3. Run API requests and validate responses

## Integration Testing

Integration tests verify the interaction between different components:

```powershell
# Run integration tests
cd C:\Projects\OuiAI\tests\Integration.Tests
dotnet test
```

## End-to-End Testing

### Running Cypress Tests (Web)

```powershell
# Navigate to web frontend directory
cd C:\Projects\OuiAI\src\Frontend\OuiAI.Web\ouiai-web

# Open Cypress Test Runner
npm run cypress:open

# Run Cypress tests headlessly
npm run cypress:run
```

### Running Detox Tests (Mobile)

```powershell
# Navigate to mobile frontend directory
cd C:\Projects\OuiAI\src\Frontend\OuiAI.Mobile\OuiAI

# Build the app for testing
detox build --configuration android.emu.debug

# Run Detox tests
detox test --configuration android.emu.debug
```

## Performance Testing

### JMeter Load Testing

1. Install Apache JMeter
2. Open the test plan from `tests/Performance/OuiAI_LoadTest.jmx`
3. Configure test parameters (users, ramp-up period, etc.)
4. Run the test and analyze results

## Security Testing

### OWASP ZAP Scanning

1. Install OWASP ZAP
2. Configure it to scan the application endpoints
3. Run the security scan
4. Review and address identified vulnerabilities

## Test Cases

### User Authentication Test Cases

1. **TC-AUTH-001: User Registration**
   - **Description**: Test user registration functionality
   - **Steps**:
     1. Navigate to the registration page
     2. Fill in valid registration details
     3. Submit the form
   - **Expected Result**: User account is created and user is redirected to login page

2. **TC-AUTH-002: User Login**
   - **Description**: Test user login functionality
   - **Steps**:
     1. Navigate to the login page
     2. Enter valid credentials
     3. Submit the form
   - **Expected Result**: User is authenticated and redirected to the dashboard

3. **TC-AUTH-003: Invalid Login**
   - **Description**: Test login validation
   - **Steps**:
     1. Navigate to the login page
     2. Enter invalid credentials
     3. Submit the form
   - **Expected Result**: Appropriate error message is displayed

4. **TC-AUTH-004: Password Reset**
   - **Description**: Test password reset functionality
   - **Steps**:
     1. Navigate to the login page
     2. Click "Forgot Password"
     3. Enter email address
     4. Follow password reset instructions
   - **Expected Result**: User receives password reset email and can reset password

### Social Features Test Cases

1. **TC-SOCIAL-001: Follow User**
   - **Description**: Test ability to follow another user
   - **Steps**:
     1. Navigate to a user's profile
     2. Click the "Follow" button
   - **Expected Result**: User is followed and button changes to "Following"

2. **TC-SOCIAL-002: Like Activity**
   - **Description**: Test ability to like a user's activity
   - **Steps**:
     1. Navigate to the social feed
     2. Click the "Like" button on an activity
   - **Expected Result**: Activity is liked and like count increases

3. **TC-SOCIAL-003: Comment on Activity**
   - **Description**: Test ability to comment on an activity
   - **Steps**:
     1. Navigate to the social feed
     2. Click on an activity to view details
     3. Add a comment and submit
   - **Expected Result**: Comment is added and visible under the activity

### Messaging Test Cases

1. **TC-MSG-001: Start New Conversation**
   - **Description**: Test creating a new conversation
   - **Steps**:
     1. Navigate to Messages
     2. Click "New Message"
     3. Select a recipient
     4. Enter message and send
   - **Expected Result**: New conversation is created with the first message sent

2. **TC-MSG-002: Reply to Message**
   - **Description**: Test replying to a message
   - **Steps**:
     1. Open an existing conversation
     2. Type a reply
     3. Send the message
   - **Expected Result**: Message is sent and appears in the conversation

3. **TC-MSG-003: Group Conversation**
   - **Description**: Test creating a group conversation
   - **Steps**:
     1. Navigate to Messages
     2. Click "New Message"
     3. Select multiple recipients
     4. Enter message and send
   - **Expected Result**: Group conversation is created with all selected users

### Notifications Test Cases

1. **TC-NOTIF-001: Receive Notification**
   - **Description**: Test receiving notifications
   - **Steps**:
     1. Have another user perform an action that triggers a notification
     2. Check notification area
   - **Expected Result**: Notification appears in real-time

2. **TC-NOTIF-002: Mark Notification as Read**
   - **Description**: Test marking notifications as read
   - **Steps**:
     1. Open notifications panel
     2. Click on a notification
   - **Expected Result**: Notification is marked as read

3. **TC-NOTIF-003: Notification Settings**
   - **Description**: Test notification settings
   - **Steps**:
     1. Navigate to user settings
     2. Adjust notification preferences
     3. Save changes
   - **Expected Result**: Notification settings are updated

### Profile Test Cases

1. **TC-PROFILE-001: Edit Profile**
   - **Description**: Test editing user profile
   - **Steps**:
     1. Navigate to user profile
     2. Click "Edit Profile"
     3. Update information
     4. Save changes
   - **Expected Result**: Profile information is updated

2. **TC-PROFILE-002: View Activity History**
   - **Description**: Test viewing user activity history
   - **Steps**:
     1. Navigate to user profile
     2. Scroll through activity list
   - **Expected Result**: User activities are displayed chronologically

3. **TC-PROFILE-003: Upload Profile Picture**
   - **Description**: Test uploading a profile picture
   - **Steps**:
     1. Navigate to user profile
     2. Click on profile picture
     3. Upload a new image
     4. Save changes
   - **Expected Result**: Profile picture is updated

## API Test Cases

1. **TC-API-001: Get User Profile**
   - **Endpoint**: `GET /api/users/{userId}`
   - **Expected Status**: 200 OK
   - **Expected Response**: User profile JSON object

2. **TC-API-002: Create Activity**
   - **Endpoint**: `POST /api/activities`
   - **Request Body**: Activity details
   - **Expected Status**: 201 Created
   - **Expected Response**: Created activity with ID

3. **TC-API-003: Get Conversations**
   - **Endpoint**: `GET /api/conversations`
   - **Expected Status**: 200 OK
   - **Expected Response**: List of user conversations

## Performance Test Cases

1. **TC-PERF-001: API Response Time**
   - **Description**: Test API response time under normal load
   - **Criteria**: 95% of requests should complete in under 500ms

2. **TC-PERF-002: Concurrent User Load**
   - **Description**: Test system performance with 100 concurrent users
   - **Criteria**: System remains responsive with response times under 2 seconds

3. **TC-PERF-003: Database Query Performance**
   - **Description**: Test database query performance
   - **Criteria**: Complex queries should complete in under 1 second

## Security Test Cases

1. **TC-SEC-001: Authentication Token Protection**
   - **Description**: Test JWT token security
   - **Checks**: Token expiration, token validation, secure storage

2. **TC-SEC-002: SQL Injection Prevention**
   - **Description**: Test protection against SQL injection
   - **Checks**: Input validation, parameterized queries

3. **TC-SEC-003: XSS Prevention**
   - **Description**: Test protection against cross-site scripting
   - **Checks**: Input sanitization, content security policy
