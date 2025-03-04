import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import {
  Box,
  Typography,
  List,
  ListItem,
  ListItemAvatar,
  ListItemText,
  Avatar,
  Paper,
  Divider,
  IconButton,
  Button,
  CircularProgress,
  Badge,
  Tabs,
  Tab
} from '@mui/material';
import {
  CheckCircle as CheckCircleIcon,
  Delete as DeleteIcon,
  Favorite as FavoriteIcon,
  Comment as CommentIcon,
  PersonAdd as PersonAddIcon,
  Star as StarIcon
} from '@mui/icons-material';
import { styled } from '@mui/material/styles';
import { socialApi } from '../../services/socialApi';
import { useAuth } from '../../context/AuthContext';

// Styled components
const NotificationItem = styled(ListItem)(({ theme, isRead }) => ({
  borderRadius: theme.shape.borderRadius,
  backgroundColor: isRead ? 'transparent' : theme.palette.action.hover,
  '&:hover': {
    backgroundColor: theme.palette.action.selected,
  },
}));

// Tab panel component
function TabPanel(props) {
  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`notification-tabpanel-${index}`}
      aria-labelledby={`notification-tab-${index}`}
      {...other}
    >
      {value === index && (
        <Box sx={{ py: 2 }}>
          {children}
        </Box>
      )}
    </div>
  );
}

// Mock notifications data
const generateMockNotifications = () => {
  const types = ['like', 'comment', 'follow', 'mention', 'system'];
  
  return Array.from({ length: 20 }, (_, i) => {
    const type = types[i % types.length];
    const isRead = Math.random() > 0.3;
    const createdAt = new Date(Date.now() - i * 3600000 * (Math.random() * 5));
    
    let content = '';
    let icon = null;
    let targetType = '';
    let targetId = '';
    
    switch (type) {
      case 'like':
        content = 'liked your post';
        icon = <FavoriteIcon color="error" />;
        targetType = 'post';
        targetId = `post-${i}`;
        break;
      case 'comment':
        content = 'commented on your post';
        icon = <CommentIcon color="primary" />;
        targetType = 'post';
        targetId = `post-${i}`;
        break;
      case 'follow':
        content = 'started following you';
        icon = <PersonAddIcon color="success" />;
        targetType = 'profile';
        targetId = `user-${i}`;
        break;
      case 'mention':
        content = 'mentioned you in a comment';
        icon = <CommentIcon color="primary" />;
        targetType = 'comment';
        targetId = `comment-${i}`;
        break;
      case 'system':
        content = 'Welcome to OuiAI Social! Complete your profile to get started.';
        icon = <StarIcon color="secondary" />;
        targetType = 'system';
        targetId = '';
        break;
      default:
        content = 'sent you a notification';
        icon = <StarIcon color="primary" />;
        targetType = 'system';
        targetId = '';
    }
    
    return {
      id: `notification-${i}`,
      type,
      content,
      isRead,
      createdAt: createdAt.toISOString(),
      user: type !== 'system' ? {
        id: `user-${i % 10}`,
        username: `user${i % 10}`,
        displayName: `User ${i % 10}`,
        profileImageUrl: `https://i.pravatar.cc/150?u=${i % 10}`,
      } : null,
      targetType,
      targetId,
      icon
    };
  });
};

/**
 * Helper function to format notification time
 */
const formatNotificationTime = (timestamp) => {
  const date = new Date(timestamp);
  const now = new Date();
  const diffInSeconds = Math.floor((now - date) / 1000);
  
  if (diffInSeconds < 60) {
    return 'Just now';
  } else if (diffInSeconds < 3600) {
    const minutes = Math.floor(diffInSeconds / 60);
    return `${minutes}m ago`;
  } else if (diffInSeconds < 86400) {
    const hours = Math.floor(diffInSeconds / 3600);
    return `${hours}h ago`;
  } else if (diffInSeconds < 604800) { // 7 days
    const days = Math.floor(diffInSeconds / 86400);
    return `${days}d ago`;
  } else {
    return date.toLocaleDateString();
  }
};

/**
 * Component for rendering a notification item
 */
const NotificationListItem = ({ notification, onMarkAsRead, onDelete }) => {
  const handleClick = () => {
    if (!notification.isRead) {
      onMarkAsRead(notification.id);
    }
  };
  
  // Generate target link based on notification type
  const getTargetLink = () => {
    switch (notification.targetType) {
      case 'post':
        return `/post/${notification.targetId}`;
      case 'comment':
        return `/post/${notification.targetId.split('-')[0]}`;
      case 'profile':
        return `/profile/${notification.user.id}`;
      default:
        return '#';
    }
  };
  
  return (
    <NotificationItem
      alignItems="flex-start"
      secondaryAction={
        <IconButton 
          edge="end" 
          aria-label="delete"
          onClick={() => onDelete(notification.id)}
        >
          <DeleteIcon />
        </IconButton>
      }
      onClick={handleClick}
      isRead={notification.isRead}
      component={notification.targetType !== 'system' ? Link : 'div'}
      to={notification.targetType !== 'system' ? getTargetLink() : undefined}
      sx={{ textDecoration: 'none', color: 'inherit' }}
    >
      <ListItemAvatar>
        {notification.type === 'system' ? (
          <Avatar sx={{ bgcolor: 'secondary.main' }}>
            {notification.icon}
          </Avatar>
        ) : (
          <Badge
            overlap="circular"
            anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
            badgeContent={notification.icon}
          >
            <Avatar 
              src={notification.user.profileImageUrl} 
              alt={notification.user.displayName} 
            />
          </Badge>
        )}
      </ListItemAvatar>
      
      <ListItemText
        primary={
          <Box sx={{ display: 'flex', alignItems: 'center' }}>
            <Typography 
              component="span" 
              variant="subtitle1" 
              color="text.primary"
              sx={{ fontWeight: notification.isRead ? 'normal' : 'bold' }}
            >
              {notification.type === 'system' ? 'OuiAI Social' : notification.user.displayName}
            </Typography>
            
            <Typography 
              component="span" 
              variant="body2" 
              color="text.secondary"
              sx={{ ml: 0.5 }}
            >
              {notification.type !== 'system' && `@${notification.user.username}`}
            </Typography>
          </Box>
        }
        secondary={
          <Box>
            <Typography 
              component="span" 
              variant="body2" 
              color="text.primary"
              sx={{ 
                display: 'inline',
                fontWeight: notification.isRead ? 'normal' : 'medium'
              }}
            >
              {notification.content}
            </Typography>
            <Typography
              component="div"
              variant="caption"
              color="text.secondary"
              sx={{ mt: 0.5 }}
            >
              {formatNotificationTime(notification.createdAt)}
            </Typography>
          </Box>
        }
      />
    </NotificationItem>
  );
};

/**
 * Notifications page component
 */
const Notifications = () => {
  const { user } = useAuth();
  const [loading, setLoading] = useState(true);
  const [notifications, setNotifications] = useState([]);
  const [tabValue, setTabValue] = useState(0);
  
  // Fetch notifications
  useEffect(() => {
    const fetchNotifications = async () => {
      setLoading(true);
      
      try {
        // In a real app, we would fetch notifications from the API
        // const response = await socialApi.notification.getNotifications();
        // setNotifications(response.data);
        
        // For now, use mock data
        setTimeout(() => {
          setNotifications(generateMockNotifications());
          setLoading(false);
          
          // Mark all as read on the server
          socialApi.notification.markAllAsRead()
            .catch(error => console.error('Error marking notifications as read:', error));
        }, 1000);
      } catch (error) {
        console.error('Error fetching notifications:', error);
        setLoading(false);
      }
    };
    
    fetchNotifications();
    
    // Subscribe to new notifications
    socialApi.subscribeToNotifications((notification) => {
      setNotifications(prev => [notification, ...prev]);
    });
  }, []);
  
  // Handle tab change
  const handleTabChange = (event, newValue) => {
    setTabValue(newValue);
  };
  
  // Mark a notification as read
  const handleMarkAsRead = async (notificationId) => {
    try {
      // In a real app, we would call the API
      // await socialApi.notification.markAsRead(notificationId);
      
      // Update the local state
      setNotifications(prevNotifications => 
        prevNotifications.map(notification => 
          notification.id === notificationId
            ? { ...notification, isRead: true }
            : notification
        )
      );
    } catch (error) {
      console.error('Error marking notification as read:', error);
    }
  };
  
  // Delete a notification
  const handleDelete = async (notificationId) => {
    try {
      // In a real app, we would call the API
      // await socialApi.notification.deleteNotification(notificationId);
      
      // Update the local state
      setNotifications(prevNotifications => 
        prevNotifications.filter(notification => notification.id !== notificationId)
      );
    } catch (error) {
      console.error('Error deleting notification:', error);
    }
  };
  
  // Mark all notifications as read
  const handleMarkAllAsRead = async () => {
    try {
      // In a real app, we would call the API
      // await socialApi.notification.markAllAsRead();
      
      // Update the local state
      setNotifications(prevNotifications => 
        prevNotifications.map(notification => ({ ...notification, isRead: true }))
      );
    } catch (error) {
      console.error('Error marking all notifications as read:', error);
    }
  };
  
  // Filter notifications based on tab
  const filteredNotifications = notifications.filter(notification => {
    if (tabValue === 0) return true; // All notifications
    if (tabValue === 1) return !notification.isRead; // Unread
    return notification.isRead; // Read
  });
  
  // Count of unread notifications
  const unreadCount = notifications.filter(notification => !notification.isRead).length;
  
  return (
    <Box sx={{ width: '100%' }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Typography variant="h5" component="h1">
          Notifications
        </Typography>
        
        <Button
          variant="outlined"
          startIcon={<CheckCircleIcon />}
          onClick={handleMarkAllAsRead}
          disabled={unreadCount === 0}
        >
          Mark all as read
        </Button>
      </Box>
      
      <Paper sx={{ width: '100%', mb: 3 }}>
        <Tabs
          value={tabValue}
          onChange={handleTabChange}
          indicatorColor="primary"
          textColor="primary"
          variant="fullWidth"
        >
          <Tab label="All" />
          <Tab 
            label={
              <Badge badgeContent={unreadCount} color="primary" max={99}>
                Unread
              </Badge>
            } 
          />
          <Tab label="Read" />
        </Tabs>
        
        <TabPanel value={tabValue} index={0}>
          {renderNotificationList()}
        </TabPanel>
        
        <TabPanel value={tabValue} index={1}>
          {renderNotificationList()}
        </TabPanel>
        
        <TabPanel value={tabValue} index={2}>
          {renderNotificationList()}
        </TabPanel>
      </Paper>
    </Box>
  );
  
  // Helper function to render the notification list
  function renderNotificationList() {
    if (loading) {
      return (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}>
          <CircularProgress />
        </Box>
      );
    }
    
    if (filteredNotifications.length === 0) {
      return (
        <Box sx={{ p: 3, textAlign: 'center' }}>
          <Typography color="textSecondary">
            {tabValue === 0 
              ? 'No notifications yet.' 
              : tabValue === 1 
              ? 'No unread notifications.' 
              : 'No read notifications.'}
          </Typography>
        </Box>
      );
    }
    
    return (
      <List>
        {filteredNotifications.map((notification) => (
          <React.Fragment key={notification.id}>
            <NotificationListItem
              notification={notification}
              onMarkAsRead={handleMarkAsRead}
              onDelete={handleDelete}
            />
            <Divider variant="inset" component="li" />
          </React.Fragment>
        ))}
      </List>
    );
  }
};

export default Notifications;
