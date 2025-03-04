import axios from 'axios';
import { HubConnectionBuilder, LogLevel, HubConnection } from '@microsoft/signalr';
import AsyncStorage from '@react-native-async-storage/async-storage';

// API base URL (should be configurable based on environment)
const API_BASE_URL = 'https://api.ouiai.com/social';
const AUTH_TOKEN_KEY = 'auth_token';

// Axios instance for social API
const socialApiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add request interceptor to add auth token to all requests
socialApiClient.interceptors.request.use(
  async (config) => {
    const token = await AsyncStorage.getItem(AUTH_TOKEN_KEY);
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// SignalR hub connections
let conversationHubConnection: HubConnection | null = null;
let notificationHubConnection: HubConnection | null = null;

// Initialize SignalR hub connections
const initializeSignalRHubs = async () => {
  const token = await AsyncStorage.getItem(AUTH_TOKEN_KEY);
  
  if (!token) {
    console.error('No auth token found for SignalR connection');
    return;
  }

  // Initialize Conversation Hub
  conversationHubConnection = new HubConnectionBuilder()
    .withUrl(`${API_BASE_URL}/hubs/conversation`, {
      accessTokenFactory: () => token,
    })
    .configureLogging(LogLevel.Information)
    .withAutomaticReconnect()
    .build();

  // Initialize Notification Hub
  notificationHubConnection = new HubConnectionBuilder()
    .withUrl(`${API_BASE_URL}/hubs/notification`, {
      accessTokenFactory: () => token,
    })
    .configureLogging(LogLevel.Information)
    .withAutomaticReconnect()
    .build();

  try {
    await conversationHubConnection.start();
    console.log('Conversation hub connected');
    
    await notificationHubConnection.start();
    console.log('Notification hub connected');
  } catch (err) {
    console.error('Error connecting to SignalR hubs:', err);
  }
};

// Disconnect SignalR hub connections
const disconnectSignalRHubs = async () => {
  if (conversationHubConnection) {
    await conversationHubConnection.stop();
    conversationHubConnection = null;
  }
  
  if (notificationHubConnection) {
    await notificationHubConnection.stop();
    notificationHubConnection = null;
  }
};

// User Follow API
const followApi = {
  followUser: (followeeId: string) => 
    socialApiClient.post('/follows', { followeeId }),
  
  unfollowUser: (followeeId: string) => 
    socialApiClient.delete(`/follows/${followeeId}`),
  
  checkFollowStatus: (followeeId: string) => 
    socialApiClient.get(`/follows/check/${followeeId}`),
  
  getFollowers: (userId: string, page = 1, pageSize = 20) => 
    socialApiClient.get(`/follows/followers/${userId}`, { params: { page, pageSize } }),
  
  getFollowing: (userId: string, page = 1, pageSize = 20) => 
    socialApiClient.get(`/follows/following/${userId}`, { params: { page, pageSize } }),
  
  getSuggestions: (page = 1, pageSize = 20) => 
    socialApiClient.get('/follows/suggestions', { params: { page, pageSize } }),
};

// Notification API
const notificationApi = {
  getNotifications: (page = 1, pageSize = 20) => 
    socialApiClient.get('/notifications', { params: { page, pageSize } }),
  
  getUnreadCount: () => 
    socialApiClient.get('/notifications/unread/count'),
  
  markAsRead: (notificationId: string) => 
    socialApiClient.put(`/notifications/${notificationId}/read`),
  
  markAllAsRead: () => 
    socialApiClient.put('/notifications/read/all'),
  
  deleteNotification: (notificationId: string) => 
    socialApiClient.delete(`/notifications/${notificationId}`),
  
  // SignalR methods
  onReceiveNotification: (callback: (notification: any) => void) => {
    if (notificationHubConnection) {
      notificationHubConnection.on('ReceiveNotification', callback);
    }
  },
};

// Activity API
const activityApi = {
  getUserActivities: (userId: string, page = 1, pageSize = 20) => 
    socialApiClient.get(`/activities/user/${userId}`, { params: { page, pageSize } }),
  
  getFollowingActivities: (page = 1, pageSize = 20) => 
    socialApiClient.get('/activities/following', { params: { page, pageSize } }),
  
  getGlobalActivities: (page = 1, pageSize = 20) => 
    socialApiClient.get('/activities', { params: { page, pageSize } }),
};

// Conversation API
const conversationApi = {
  getConversations: (page = 1, pageSize = 20) => 
    socialApiClient.get('/conversations', { params: { page, pageSize } }),
  
  getConversation: (conversationId: string) => 
    socialApiClient.get(`/conversations/${conversationId}`),
  
  createConversation: (participantIds: string[]) => 
    socialApiClient.post('/conversations', { participantIds }),
  
  addParticipant: (conversationId: string, userId: string) => 
    socialApiClient.post(`/conversations/${conversationId}/participants`, { userId }),
  
  removeParticipant: (conversationId: string, userId: string) => 
    socialApiClient.delete(`/conversations/${conversationId}/participants/${userId}`),
  
  markConversationAsRead: (conversationId: string) => 
    socialApiClient.put(`/conversations/${conversationId}/read`),
  
  getUnreadCount: () => 
    socialApiClient.get('/conversations/unread/count'),
  
  // SignalR methods
  onConversationUpdated: (callback: (conversation: any) => void) => {
    if (conversationHubConnection) {
      conversationHubConnection.on('ConversationUpdated', callback);
    }
  },
  
  onUserJoinedGroup: (callback: (conversationId: string, user: any) => void) => {
    if (conversationHubConnection) {
      conversationHubConnection.on('UserJoinedGroup', callback);
    }
  },
  
  onUserLeftGroup: (callback: (conversationId: string, userId: string) => void) => {
    if (conversationHubConnection) {
      conversationHubConnection.on('UserLeftGroup', callback);
    }
  },
};

// Message API
const messageApi = {
  getMessages: (conversationId: string, page = 1, pageSize = 20) => 
    socialApiClient.get(`/conversations/${conversationId}/messages`, { params: { page, pageSize } }),
  
  sendMessage: (conversationId: string, content: string) => 
    socialApiClient.post(`/conversations/${conversationId}/messages`, { content }),
  
  deleteMessage: (messageId: string) => 
    socialApiClient.delete(`/messages/${messageId}`),
  
  markMessageAsRead: (messageId: string) => 
    socialApiClient.put(`/messages/${messageId}/read`),
  
  // SignalR methods
  onReceiveMessage: (callback: (message: any) => void) => {
    if (conversationHubConnection) {
      conversationHubConnection.on('ReceiveMessage', callback);
    }
  },
  
  onMessageDeleted: (callback: (messageId: string) => void) => {
    if (conversationHubConnection) {
      conversationHubConnection.on('MessageDeleted', callback);
    }
  },
  
  onUserTyping: (callback: (conversationId: string, user: any) => void) => {
    if (conversationHubConnection) {
      conversationHubConnection.on('UserTyping', callback);
    }
  },
  
  sendTypingIndicator: (conversationId: string) => {
    if (conversationHubConnection && conversationHubConnection.state === 'Connected') {
      conversationHubConnection.invoke('SendTypingIndicator', conversationId);
    }
  },
};

// Export all APIs as socialApi
export const socialApi = {
  follow: followApi,
  notification: notificationApi,
  activity: activityApi,
  conversation: conversationApi,
  message: messageApi,
  initialize: initializeSignalRHubs,
  disconnect: disconnectSignalRHubs,
};

export default socialApi;
