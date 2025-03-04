import React, { createContext, useState, useContext, useEffect } from 'react';
import axios from 'axios';

// API base URL (should be configurable based on environment)
const AUTH_API_URL = 'https://api.ouiai.com/identity';
const AUTH_TOKEN_KEY = 'auth_token';
const USER_DATA_KEY = 'user_data';

// Create the auth context
const AuthContext = createContext();

// Auth context provider component
export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [isLoading, setIsLoading] = useState(true);

  // Check for existing authentication on app start
  useEffect(() => {
    const loadStoredAuth = async () => {
      try {
        const storedUser = localStorage.getItem(USER_DATA_KEY);
        const token = localStorage.getItem(AUTH_TOKEN_KEY);

        if (storedUser && token) {
          setUser(JSON.parse(storedUser));
          
          // Set axios defaults for future requests
          axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
        }
      } catch (error) {
        console.error('Error loading stored auth:', error);
      } finally {
        setIsLoading(false);
      }
    };

    loadStoredAuth();
  }, []);

  // Login function
  const login = async (email, password) => {
    setIsLoading(true);
    try {
      const response = await axios.post(`${AUTH_API_URL}/auth/login`, {
        email,
        password,
      });
      
      const { token, user } = response.data;
      
      // Store authentication data
      localStorage.setItem(AUTH_TOKEN_KEY, token);
      localStorage.setItem(USER_DATA_KEY, JSON.stringify(user));
      
      // Set user in state
      setUser(user);
      
      // Set axios defaults for future requests
      axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
      
      return true;
    } catch (error) {
      console.error('Login error:', error);
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  // Register function
  const register = async (username, email, password, displayName) => {
    setIsLoading(true);
    try {
      await axios.post(`${AUTH_API_URL}/auth/register`, {
        username,
        email,
        password,
        displayName,
      });
      
      // Automatically login after successful registration
      return await login(email, password);
    } catch (error) {
      console.error('Registration error:', error);
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  // Logout function
  const logout = async () => {
    setIsLoading(true);
    try {
      // Clear stored auth data
      localStorage.removeItem(AUTH_TOKEN_KEY);
      localStorage.removeItem(USER_DATA_KEY);
      
      // Clear user state
      setUser(null);
      
      // Remove authorization header
      delete axios.defaults.headers.common['Authorization'];
      
      return true;
    } catch (error) {
      console.error('Logout error:', error);
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  // Update user data
  const updateUser = async (userData) => {
    try {
      // Update user data on the server
      const response = await axios.put(
        `${AUTH_API_URL}/users/profile`,
        userData
      );
      
      // Update local storage and state
      const updatedUser = { ...user, ...response.data };
      localStorage.setItem(USER_DATA_KEY, JSON.stringify(updatedUser));
      setUser(updatedUser);
      
      return true;
    } catch (error) {
      console.error('Update user error:', error);
      return false;
    }
  };

  // Context value
  const value = {
    user,
    isAuthenticated: !!user,
    isLoading,
    login,
    register,
    logout,
    updateUser,
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};

// Custom hook to use the auth context
export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

export default AuthContext;
