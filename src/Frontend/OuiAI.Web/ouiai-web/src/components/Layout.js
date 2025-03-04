import React, { useState, useEffect } from 'react';
import { Outlet, Link, useLocation, useNavigate } from 'react-router-dom';
import { styled } from '@mui/material/styles';
import {
  AppBar,
  Toolbar,
  Typography,
  IconButton,
  Drawer,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Badge,
  Avatar,
  Menu,
  MenuItem,
  Divider,
  Box,
  Container
} from '@mui/material';
import {
  Menu as MenuIcon,
  Home as HomeIcon,
  People as PeopleIcon,
  Message as MessageIcon,
  Notifications as NotificationsIcon,
  AccountCircle as AccountCircleIcon,
  ExitToApp as LogoutIcon
} from '@mui/icons-material';
import { useAuth } from '../context/AuthContext';
import { socialApi } from '../services/socialApi';

// Styled components
const drawerWidth = 240;

const Main = styled('main', { shouldForwardProp: (prop) => prop !== 'open' })(
  ({ theme, open }) => ({
    flexGrow: 1,
    padding: theme.spacing(3),
    transition: theme.transitions.create('margin', {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.leavingScreen,
    }),
    marginLeft: 0,
    ...(open && {
      transition: theme.transitions.create('margin', {
        easing: theme.transitions.easing.easeOut,
        duration: theme.transitions.duration.enteringScreen,
      }),
      marginLeft: drawerWidth,
    }),
  }),
);

const DrawerHeader = styled('div')(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  padding: theme.spacing(0, 1),
  ...theme.mixins.toolbar,
  justifyContent: 'center',
}));

// Layout component that provides the app shell with navigation
const Layout = () => {
  const { user, logout } = useAuth();
  const location = useLocation();
  const navigate = useNavigate();
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [anchorEl, setAnchorEl] = useState(null);
  const [notificationCount, setNotificationCount] = useState(0);
  const [messageCount, setMessageCount] = useState(0);
  
  // Handle opening/closing drawer
  const toggleDrawer = () => {
    setDrawerOpen(!drawerOpen);
  };

  // Handle opening/closing user menu
  const handleMenu = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleCloseMenu = () => {
    setAnchorEl(null);
  };

  // Handle logout
  const handleLogout = async () => {
    handleCloseMenu();
    await logout();
    navigate('/login');
  };

  // Handle profile navigation
  const handleProfile = () => {
    handleCloseMenu();
    navigate('/profile');
  };

  // Get notification and message counts on mount
  useEffect(() => {
    const fetchCounts = async () => {
      try {
        // Get unread notifications count
        const notificationsResponse = await socialApi.notification.getNotifications();
        const unreadNotifications = notificationsResponse.data.filter(n => !n.isRead);
        setNotificationCount(unreadNotifications.length);
        
        // Get unread messages count
        const conversationsResponse = await socialApi.conversation.getConversations();
        const unreadMessages = conversationsResponse.data.reduce(
          (count, conv) => count + (conv.unreadCount || 0), 
          0
        );
        setMessageCount(unreadMessages);
      } catch (error) {
        console.error('Error fetching notification/message counts:', error);
      }
    };
    
    fetchCounts();
    
    // Set up subscription for real-time notification updates
    socialApi.subscribeToNotifications((notification) => {
      setNotificationCount(prev => prev + 1);
    });
    
    // Set up subscription for real-time message updates
    socialApi.subscribeToMessages((message) => {
      // Only increment if the message is not from the current user
      if (message.senderId !== user?.id) {
        setMessageCount(prev => prev + 1);
      }
    });
  }, [user?.id]);
  
  // Reset notification count when navigating to notifications page
  useEffect(() => {
    if (location.pathname === '/notifications' && notificationCount > 0) {
      socialApi.notification.markAllAsRead()
        .then(() => setNotificationCount(0))
        .catch(error => console.error('Error marking notifications as read:', error));
    }
  }, [location.pathname, notificationCount]);

  // Navigation items
  const navigationItems = [
    { text: 'Home', icon: <HomeIcon />, path: '/' },
    { text: 'Social', icon: <PeopleIcon />, path: '/social' },
    { 
      text: 'Messages', 
      icon: 
        <Badge badgeContent={messageCount} color="error">
          <MessageIcon />
        </Badge>, 
      path: '/messages' 
    },
    { 
      text: 'Notifications', 
      icon: 
        <Badge badgeContent={notificationCount} color="error">
          <NotificationsIcon />
        </Badge>, 
      path: '/notifications' 
    },
  ];

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh' }}>
      {/* App bar */}
      <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
        <Toolbar>
          <IconButton
            color="inherit"
            aria-label="open drawer"
            edge="start"
            onClick={toggleDrawer}
            sx={{ mr: 2 }}
          >
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap component="div" sx={{ flexGrow: 1 }}>
            OuiAI Social
          </Typography>
          
          {/* User menu */}
          <div>
            <IconButton
              size="large"
              aria-label="account of current user"
              aria-controls="menu-appbar"
              aria-haspopup="true"
              onClick={handleMenu}
              color="inherit"
            >
              {user?.profileImageUrl ? (
                <Avatar src={user.profileImageUrl} alt={user.displayName} />
              ) : (
                <AccountCircleIcon />
              )}
            </IconButton>
            <Menu
              id="menu-appbar"
              anchorEl={anchorEl}
              anchorOrigin={{
                vertical: 'bottom',
                horizontal: 'right',
              }}
              keepMounted
              transformOrigin={{
                vertical: 'top',
                horizontal: 'right',
              }}
              open={Boolean(anchorEl)}
              onClose={handleCloseMenu}
            >
              <MenuItem onClick={handleProfile}>Profile</MenuItem>
              <Divider />
              <MenuItem onClick={handleLogout}>Logout</MenuItem>
            </Menu>
          </div>
        </Toolbar>
      </AppBar>
      
      {/* Navigation drawer */}
      <Drawer
        sx={{
          width: drawerWidth,
          flexShrink: 0,
          '& .MuiDrawer-paper': {
            width: drawerWidth,
            boxSizing: 'border-box',
          },
        }}
        variant="persistent"
        anchor="left"
        open={drawerOpen}
      >
        <DrawerHeader>
          <Typography variant="h6">
            Navigation
          </Typography>
        </DrawerHeader>
        <Divider />
        <List>
          {navigationItems.map((item) => (
            <ListItem 
              button 
              key={item.text} 
              component={Link} 
              to={item.path}
              selected={location.pathname === item.path}
            >
              <ListItemIcon>{item.icon}</ListItemIcon>
              <ListItemText primary={item.text} />
            </ListItem>
          ))}
        </List>
        <Divider />
        <List>
          <ListItem button onClick={handleLogout}>
            <ListItemIcon><LogoutIcon /></ListItemIcon>
            <ListItemText primary="Logout" />
          </ListItem>
        </List>
      </Drawer>
      
      {/* Main content */}
      <Main open={drawerOpen}>
        <DrawerHeader /> {/* Spacer for toolbar */}
        <Container>
          <Outlet /> {/* Render child routes */}
        </Container>
      </Main>
    </Box>
  );
};

export default Layout;
