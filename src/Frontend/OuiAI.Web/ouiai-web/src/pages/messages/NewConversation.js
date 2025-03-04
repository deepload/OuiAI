import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import {
  Box,
  Typography,
  Paper,
  TextField,
  Button,
  List,
  ListItem,
  ListItemAvatar,
  ListItemText,
  Avatar,
  Divider,
  IconButton,
  AppBar,
  Toolbar,
  Chip,
  CircularProgress,
  InputAdornment
} from '@mui/material';
import {
  Search as SearchIcon,
  ArrowBack as ArrowBackIcon,
  Close as CloseIcon,
  Send as SendIcon
} from '@mui/icons-material';
import { socialApi } from '../../services/socialApi';
import { useAuth } from '../../context/AuthContext';

// Mock users data
const mockUsers = Array.from({ length: 20 }, (_, i) => ({
  id: `user-${i}`,
  username: `user${i}`,
  displayName: `User ${i}`,
  profileImageUrl: `https://i.pravatar.cc/150?u=${i}`,
  bio: `This is the bio for User ${i}. They are interested in technology, AI, and social networking.`
}));

/**
 * New Conversation component for creating new messages
 */
const NewConversation = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { user } = useAuth();
  const [loading, setLoading] = useState(false);
  const [userSearchTerm, setUserSearchTerm] = useState('');
  const [searchResults, setSearchResults] = useState([]);
  const [selectedUsers, setSelectedUsers] = useState([]);
  const [initialMessage, setInitialMessage] = useState('');
  const [sending, setSending] = useState(false);
  
  // Check if there's a userId in the query params
  useEffect(() => {
    const params = new URLSearchParams(location.search);
    const userId = params.get('userId');
    
    if (userId) {
      // Find user in mock data
      const user = mockUsers.find(u => u.id === userId);
      if (user) {
        setSelectedUsers([user]);
      }
      
      // In a real app, we would fetch the user data from the API
      // socialApi.user.getUserProfile(userId)
      //   .then(response => {
      //     setSelectedUsers([response.data]);
      //   })
      //   .catch(error => {
      //     console.error('Error fetching user:', error);
      //   });
    }
  }, [location.search]);
  
  // Search for users
  useEffect(() => {
    // Only search if we have at least 2 characters
    if (userSearchTerm.length < 2) {
      setSearchResults([]);
      return;
    }
    
    setLoading(true);
    
    // In a real app, we would fetch search results from the API
    // socialApi.user.searchUsers(userSearchTerm)
    //   .then(response => {
    //     setSearchResults(response.data);
    //     setLoading(false);
    //   })
    //   .catch(error => {
    //     console.error('Error searching users:', error);
    //     setLoading(false);
    //   });
    
    // For now, use mock data
    setTimeout(() => {
      const results = mockUsers.filter(user => 
        user.displayName.toLowerCase().includes(userSearchTerm.toLowerCase()) ||
        user.username.toLowerCase().includes(userSearchTerm.toLowerCase())
      );
      
      setSearchResults(results);
      setLoading(false);
    }, 500);
  }, [userSearchTerm]);
  
  // Navigate back
  const handleBack = () => {
    navigate('/messages');
  };
  
  // Select a user from search results
  const handleSelectUser = (user) => {
    // Don't add if already selected
    if (selectedUsers.some(u => u.id === user.id)) {
      return;
    }
    
    setSelectedUsers([...selectedUsers, user]);
    setUserSearchTerm('');
    setSearchResults([]);
  };
  
  // Remove a selected user
  const handleRemoveUser = (userId) => {
    setSelectedUsers(selectedUsers.filter(u => u.id !== userId));
  };
  
  // Send the initial message to create the conversation
  const handleSendMessage = async () => {
    if (selectedUsers.length === 0 || !initialMessage.trim()) {
      return;
    }
    
    setSending(true);
    
    try {
      // Get user IDs of selected users
      const participantIds = selectedUsers.map(u => u.id);
      
      // In a real app, we would create the conversation via the API
      // const response = await socialApi.conversation.createConversation(participantIds, initialMessage);
      // navigate(`/messages/${response.data.id}`);
      
      // For now, just simulate a successful API call
      setTimeout(() => {
        const mockConversationId = `conversation-${Date.now()}`;
        navigate(`/messages/${mockConversationId}`);
      }, 1000);
    } catch (error) {
      console.error('Error creating conversation:', error);
      setSending(false);
    }
  };
  
  // Check if we can proceed
  const canProceed = selectedUsers.length > 0 && initialMessage.trim().length > 0;
  
  return (
    <Box sx={{ height: 'calc(100vh - 170px)', display: 'flex', flexDirection: 'column' }}>
      {/* Header */}
      <AppBar position="static" color="default" elevation={0}>
        <Toolbar>
          <IconButton edge="start" color="inherit" onClick={handleBack} sx={{ mr: 1 }}>
            <ArrowBackIcon />
          </IconButton>
          
          <Typography variant="h6" noWrap component="div" sx={{ flexGrow: 1 }}>
            New Message
          </Typography>
        </Toolbar>
      </AppBar>
      
      <Paper 
        elevation={0} 
        sx={{ 
          flexGrow: 1, 
          display: 'flex', 
          flexDirection: 'column',
          overflow: 'hidden'
        }}
      >
        {/* Selected users */}
        <Box sx={{ p: 2, display: 'flex', flexWrap: 'wrap', gap: 1 }}>
          {selectedUsers.map(user => (
            <Chip
              key={user.id}
              avatar={<Avatar src={user.profileImageUrl} alt={user.displayName} />}
              label={user.displayName}
              onDelete={() => handleRemoveUser(user.id)}
              deleteIcon={<CloseIcon />}
              sx={{ mb: 1 }}
            />
          ))}
          
          {selectedUsers.length === 0 && (
            <Typography color="textSecondary">
              Select recipients
            </Typography>
          )}
        </Box>
        
        <Divider />
        
        {/* User search */}
        <Box sx={{ p: 2 }}>
          <TextField
            fullWidth
            placeholder="Search for users..."
            value={userSearchTerm}
            onChange={(e) => setUserSearchTerm(e.target.value)}
            variant="outlined"
            size="small"
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <SearchIcon />
                </InputAdornment>
              ),
              endAdornment: loading && (
                <InputAdornment position="end">
                  <CircularProgress size={20} />
                </InputAdornment>
              )
            }}
          />
        </Box>
        
        {/* Search results */}
        {searchResults.length > 0 && (
          <Box sx={{ maxHeight: '200px', overflow: 'auto' }}>
            <List>
              {searchResults.map(user => (
                <ListItem 
                  key={user.id} 
                  button 
                  onClick={() => handleSelectUser(user)}
                  selected={selectedUsers.some(u => u.id === user.id)}
                >
                  <ListItemAvatar>
                    <Avatar src={user.profileImageUrl} alt={user.displayName} />
                  </ListItemAvatar>
                  <ListItemText
                    primary={user.displayName}
                    secondary={`@${user.username}`}
                  />
                </ListItem>
              ))}
            </List>
          </Box>
        )}
        
        {/* Message input */}
        <Box sx={{ p: 2, mt: 'auto' }}>
          <TextField
            fullWidth
            placeholder="Write your message..."
            value={initialMessage}
            onChange={(e) => setInitialMessage(e.target.value)}
            variant="outlined"
            multiline
            rows={4}
            disabled={selectedUsers.length === 0 || sending}
          />
          
          <Box sx={{ display: 'flex', justifyContent: 'flex-end', mt: 2 }}>
            <Button
              variant="contained"
              startIcon={<SendIcon />}
              onClick={handleSendMessage}
              disabled={!canProceed || sending}
            >
              {sending ? 'Sending...' : 'Send Message'}
            </Button>
          </Box>
        </Box>
      </Paper>
    </Box>
  );
};

export default NewConversation;
