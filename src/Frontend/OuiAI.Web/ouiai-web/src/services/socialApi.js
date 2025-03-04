import axios from 'axios';
import * as signalR from '@microsoft/signalr';

// Base URL for the Social microservice API
const SOCIAL_API_URL = 'https://api.ouiai.com/social';

// Create an axios instance for social API requests
const socialApiClient = axios.create({
  baseURL: SOCIAL_API_URL,
});

// Add auth token to all requests
socialApiClient.interceptors.request.use(
  async (config) => {
    const token = localStorage.getItem('auth_token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// SignalR connection for real-time updates
let notificationConnection = null;
let messageConnection = null;

// Social API service
export const socialApi = {
  // Initialize SignalR connections
  initialize: async () => {
    const token = localStorage.getItem('auth_token');
    
    if (!token) {
      console.error('Cannot initialize SignalR connections without auth token');
      return;
    }
    
    try {
      // Create notification hub connection
      notificationConnection = new signalR.HubConnectionBuilder()
        .withUrl(`${SOCIAL_API_URL}/notificationHub`, {
          accessTokenFactory: () => token,
        })
        .withAutomaticReconnect()
        .build();
        
      // Create message hub connection
      messageConnection = new signalR.HubConnectionBuilder()
        .withUrl(`${SOCIAL_API_URL}/messageHub`, {
          accessTokenFactory: () => token,
        })
        .withAutomaticReconnect()
        .build();
      
      // Start connections
      await notificationConnection.start();
      await messageConnection.start();
      
      console.log('SignalR connections established');
    } catch (error) {
      console.error('Error initializing SignalR connections:', error);
    }
  },
  
  // Close SignalR connections
  disconnect: async () => {
    try {
      if (notificationConnection) {
        await notificationConnection.stop();
      }
      
      if (messageConnection) {
        await messageConnection.stop();
      }
      
      console.log('SignalR connections closed');
    } catch (error) {
      console.error('Error closing SignalR connections:', error);
    }
  },
  
  // Subscribe to notifications
  subscribeToNotifications: (callback) => {
    if (!notificationConnection) {
      console.error('Notification connection not initialized');
      return;
    }
    
    notificationConnection.on('ReceiveNotification', (notification) => {
      callback(notification);
    });
  },
  
  // Subscribe to messages
  subscribeToMessages: (callback) => {
    if (!messageConnection) {
      console.error('Message connection not initialized');
      return;
    }
    
    messageConnection.on('ReceiveMessage', (message) => {
      callback(message);
    });
  },
  
  // Subscribe to typing indicators
  subscribeToTypingIndicator: (callback) => {
    if (!messageConnection) {
      console.error('Message connection not initialized');
      return;
    }
    
    messageConnection.on('UserTyping', (data) => {
      callback(data);
    });
  },
  
  // Send typing indicator
  sendTypingIndicator: async (conversationId) => {
    if (!messageConnection) {
      console.error('Message connection not initialized');
      return;
    }
    
    try {
      await messageConnection.invoke('SendTypingIndicator', conversationId);
    } catch (error) {
      console.error('Error sending typing indicator:', error);
    }
  },
  
  // API endpoints for follows
  follow: {
    // Follow a user
    followUser: (followeeId) => socialApiClient.post('/follows', { followeeId }),
    
    // Unfollow a user
    unfollowUser: (followeeId) => socialApiClient.delete(`/follows/${followeeId}`),
    
    // Get followers of a user
    getFollowers: (userId, page = 1, pageSize = 20) => 
      socialApiClient.get(`/follows/followers/${userId}`, { params: { page, pageSize } }),
    
    // Get users that a user is following
    getFollowing: (userId, page = 1, pageSize = 20) => 
      socialApiClient.get(`/follows/following/${userId}`, { params: { page, pageSize } }),
  },
  
  // API endpoints for notifications
  notification: {
    // Get notifications
    getNotifications: (page = 1, pageSize = 20) => 
      socialApiClient.get('/notifications', { params: { page, pageSize } }),
    
    // Mark a notification as read
    markAsRead: (notificationId) => 
      socialApiClient.put(`/notifications/${notificationId}/read`),
    
    // Mark all notifications as read
    markAllAsRead: () => 
      socialApiClient.put('/notifications/read-all'),
  },
  
  // API endpoints for activities
  activity: {
    // Get user activities
    getUserActivities: (userId, page = 1, pageSize = 20) => 
      socialApiClient.get(`/activities/user/${userId}`, { params: { page, pageSize } }),
    
    // Get activities from followed users
    getFollowingActivities: (page = 1, pageSize = 20) => 
      socialApiClient.get('/activities/following', { params: { page, pageSize } }),
    
    // Get global activities
    getGlobalActivities: (page = 1, pageSize = 20) => 
      socialApiClient.get('/activities/global', { params: { page, pageSize } }),
  },
  
  // API endpoints for conversations
  conversation: {
    // Get conversations
    getConversations: (page = 1, pageSize = 20) => 
      socialApiClient.get('/conversations', { params: { page, pageSize } }),
    
    // Get a specific conversation
    getConversation: (conversationId) => 
      socialApiClient.get(`/conversations/${conversationId}`),
    
    // Create a new conversation
    createConversation: (participants) => 
      socialApiClient.post('/conversations', { participants }),
  },
  
  // API endpoints for messages
  message: {
    // Get messages in a conversation
    getMessages: (conversationId, page = 1, pageSize = 20) => 
      socialApiClient.get(`/messages/${conversationId}`, { params: { page, pageSize } }),
    
    // Send a message
    sendMessage: (conversationId, content) => 
      socialApiClient.post('/messages', { conversationId, content }),
  },
};
