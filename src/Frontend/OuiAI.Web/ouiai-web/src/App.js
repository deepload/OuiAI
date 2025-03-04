import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { AuthProvider, useAuth } from './context/AuthContext';
import './App.css';

// Lazy load pages
import React, { Suspense, useEffect } from 'react';
import { socialApi } from './services/socialApi';

// Layout components
const Layout = React.lazy(() => import('./components/Layout'));
const PrivateRoute = React.lazy(() => import('./components/PrivateRoute'));

// Pages
const Login = React.lazy(() => import('./pages/Login'));
const Register = React.lazy(() => import('./pages/Register'));
const Social = React.lazy(() => import('./pages/social/Social'));
const Profile = React.lazy(() => import('./pages/profile/Profile'));
const Messages = React.lazy(() => import('./pages/messages/Messages'));
const Conversation = React.lazy(() => import('./pages/messages/Conversation'));
const NewConversation = React.lazy(() => import('./pages/messages/NewConversation'));
const Notifications = React.lazy(() => import('./pages/notifications/Notifications'));

// Create theme
const theme = createTheme({
  palette: {
    primary: {
      main: '#1976d2',
    },
    secondary: {
      main: '#dc004e',
    },
  },
});

// Loading component
const Loading = () => (
  <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
    <p>Loading...</p>
  </div>
);

// Main app component with routing
function AppContent() {
  const { isAuthenticated, user } = useAuth();
  
  // Initialize SignalR connections when authenticated
  useEffect(() => {
    const initializeSignalR = async () => {
      if (isAuthenticated) {
        await socialApi.initialize();
        
        // Subscribe to notifications
        socialApi.subscribeToNotifications((notification) => {
          console.log('New notification:', notification);
          // We would update state or show a notification here
        });
      }
    };
    
    initializeSignalR();
    
    // Clean up on unmount
    return () => {
      if (isAuthenticated) {
        socialApi.disconnect();
      }
    };
  }, [isAuthenticated]);
  
  return (
    <Router>
      <Suspense fallback={<Loading />}>
        <Routes>
          {/* Public routes */}
          <Route 
            path="/login" 
            element={!isAuthenticated ? <Login /> : <Navigate to="/" replace />} 
          />
          <Route 
            path="/register" 
            element={!isAuthenticated ? <Register /> : <Navigate to="/" replace />} 
          />
          
          {/* Protected routes inside Layout */}
          <Route element={<PrivateRoute />}>
            <Route element={<Layout />}>
              <Route path="/" element={<Social />} />
              <Route path="/social" element={<Social />} />
              <Route path="/profile/:id" element={<Profile />} />
              <Route path="/profile" element={<Profile />} />
              <Route path="/messages" element={<Messages />} />
              <Route path="/messages/new" element={<NewConversation />} />
              <Route path="/messages/:id" element={<Conversation />} />
              <Route path="/notifications" element={<Notifications />} />
            </Route>
          </Route>
        </Routes>
      </Suspense>
    </Router>
  );
}

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <AuthProvider>
        <AppContent />
      </AuthProvider>
    </ThemeProvider>
  );
}

export default App;
