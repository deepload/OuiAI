# Frontend Architecture

This document outlines the architecture of the OuiAI frontend applications, covering both web and mobile platforms.

## Overview

OuiAI utilizes a shared component approach to maintain consistency across web and mobile platforms while optimizing for each platform's unique characteristics.

## Technology Stack

### Web Frontend (OuiAI.Web)

- **Framework**: React 18
- **State Management**: Redux with Redux Toolkit
- **Routing**: React Router 6
- **HTTP Client**: Axios
- **Real-time Communication**: SignalR client
- **UI Components**: Material-UI
- **Forms**: Formik with Yup validation
- **Testing**: Jest, React Testing Library
- **Building**: Webpack, Babel

### Mobile Frontend (OuiAI.Mobile)

- **Framework**: React Native with Expo
- **State Management**: Redux with Redux Toolkit
- **Navigation**: React Navigation 6
- **HTTP Client**: Axios
- **Real-time Communication**: SignalR client for React Native
- **UI Components**: React Native Paper
- **Forms**: Formik with Yup validation
- **Testing**: Jest, React Native Testing Library
- **Building**: Expo Build Services

## Application Structure

### Code Organization

Both web and mobile applications follow a similar code organization pattern:

```
frontend/
├── src/
│   ├── api/               # API service clients
│   ├── assets/            # Static assets (images, fonts)
│   ├── components/        # Shared UI components
│   │   ├── common/        # Cross-platform components
│   │   ├── forms/         # Form components
│   │   └── layout/        # Layout components
│   ├── constants/         # Application constants
│   ├── contexts/          # React contexts
│   ├── hooks/             # Custom React hooks
│   ├── models/            # TypeScript interfaces/types
│   ├── navigation/        # Navigation configuration
│   ├── pages/             # Page components (web)
│   ├── screens/           # Screen components (mobile)
│   ├── services/          # Business logic services
│   ├── store/             # Redux store configuration
│   │   ├── slices/        # Redux toolkit slices
│   │   ├── middleware/    # Redux middleware
│   │   └── selectors/     # Redux selectors
│   ├── styles/            # Global styles
│   ├── theme/             # Theme configuration
│   └── utils/             # Utility functions
└── tests/                 # Test files
```

## State Management

OuiAI uses Redux for global state management with the following slices:

### Auth Slice
Manages authentication state and user information:
- User authentication status
- User profile data
- JWT token management
- Auth-related error handling

### Projects Slice
Manages project-related state:
- Project listings
- Project details
- Project creation and editing
- Project filtering

### Social Slice
Manages social interaction state:
- User connections
- Activity feed
- Likes, comments, shares
- Notifications

### UI Slice
Manages UI-related state:
- Theme (dark/light mode)
- UI preferences
- Modal dialogs
- Loading indicators

## Component Architecture

### Component Hierarchy

1. **App Container**: Root component that sets up routing, theme, and global providers
2. **Layout Components**: Page/screen layouts with navigation, headers, footers
3. **Feature Components**: Components specific to business features
4. **Common Components**: Reusable UI components
5. **Base Components**: Primitive UI components with styling

### Component Design Principles

1. **Composition Over Inheritance**: Components are composed rather than inherited
2. **Container/Presentational Pattern**: Separate logic from UI
3. **Custom Hooks**: Extract reusable logic into custom hooks
4. **Prop Drilling Avoidance**: Use context or Redux for deeply nested state

## API Communication

### API Client

The API client is structured by domain:

```typescript
// Base API client with common configuration
const apiClient = axios.create({
  baseURL: API_URL,
  timeout: 10000,
  headers: { 'Content-Type': 'application/json' }
});

// Domain-specific API services
const authApi = {
  login: (credentials) => apiClient.post('/auth/login', credentials),
  register: (userData) => apiClient.post('/auth/register', userData),
  // ...
};

const projectsApi = {
  getProjects: (filters) => apiClient.get('/projects', { params: filters }),
  getProjectById: (id) => apiClient.get(`/projects/${id}`),
  // ...
};
```

### Request/Response Interceptors

API clients use interceptors for:
- Adding authentication tokens to requests
- Refreshing expired tokens
- Handling common error responses
- Logging for debugging

## Authentication Flow

1. **Login/Registration**: User credentials sent to Identity Service
2. **Token Reception**: JWT token received and stored
3. **Token Storage**: JWT stored in secure storage (localStorage for web, SecureStore for mobile)
4. **Request Authentication**: Token attached to API requests
5. **Token Refresh**: Automatic token refresh when expired
6. **Logout**: Token removal and state cleanup

## Navigation & Routing

### Web Routing

Uses React Router with route configuration:

```jsx
<Routes>
  <Route element={<PublicLayout />}>
    <Route path="/login" element={<Login />} />
    <Route path="/register" element={<Register />} />
  </Route>
  <Route element={<ProtectedLayout />}>
    <Route path="/" element={<Home />} />
    <Route path="/projects" element={<Projects />} />
    <Route path="/projects/:id" element={<ProjectDetail />} />
    <Route path="/messages" element={<Messages />} />
    <Route path="/profile" element={<Profile />} />
  </Route>
</Routes>
```

### Mobile Navigation

Uses React Navigation with navigation hierarchy:

```jsx
<NavigationContainer>
  <Stack.Navigator>
    {!isAuthenticated ? (
      // Auth Stack
      <Stack.Screen name="Auth" component={AuthNavigator} />
    ) : (
      // Main App Stack with Bottom Tabs
      <Stack.Screen name="Main" component={MainTabNavigator} />
    )}
  </Stack.Navigator>
</NavigationContainer>
```

## Forms and Validation

OuiAI uses Formik for form management with Yup schema validation:

```jsx
const validationSchema = Yup.object().shape({
  email: Yup.string()
    .email('Invalid email')
    .required('Email is required'),
  password: Yup.string()
    .min(8, 'Password must be at least 8 characters')
    .required('Password is required')
});

function LoginForm() {
  return (
    <Formik
      initialValues={{ email: '', password: '' }}
      validationSchema={validationSchema}
      onSubmit={handleLogin}
    >
      {/* Form fields */}
    </Formik>
  );
}
```

## Error Handling

1. **API Error Handling**: Centralized error handling for API requests
2. **Global Error Boundary**: Catches and displays UI errors
3. **Form Validation Errors**: Displayed inline with form fields
4. **Toast Notifications**: Non-blocking error messages
5. **Offline Handling**: Graceful degradation when offline

## Theming and Styling

### Web Styling

- Material-UI theming
- CSS-in-JS with emotion
- Responsive design with breakpoints
- Dark mode support

### Mobile Styling

- React Native Paper theming
- StyleSheet for component styles
- Responsive sizing with Dimensions API
- Dark mode support

## Performance Optimization

1. **Code Splitting**: Lazy loading of routes/screens
2. **Memoization**: React.memo, useMemo, useCallback
3. **Virtual Lists**: FlatList/VirtualizedList for long lists
4. **Image Optimization**: Responsive images, lazy loading
5. **Bundle Size Optimization**: Tree-shaking, dead code elimination

## Testing Strategy

1. **Unit Tests**: Testing individual components and functions
2. **Integration Tests**: Testing component interactions
3. **E2E Tests**: Testing complete user flows
4. **Component Snapshots**: Visual regression testing
5. **Accessibility Testing**: Ensuring accessibility compliance

## Deployment

### Web Deployment

- Static file hosting on Azure Storage
- Azure CDN for content delivery
- Environment-specific builds (dev, staging, prod)
- Automated deployment via CI/CD

### Mobile Deployment

- App Store (iOS) and Google Play (Android) distribution
- Expo EAS Build for app builds
- Over-the-air updates for minor changes
- Environment-specific configurations
