import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
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
  TextField,
  InputAdornment,
  IconButton,
  Badge,
  Button,
  CircularProgress
} from '@mui/material';
import {
  Search as SearchIcon,
  Edit as EditIcon,
  Circle as CircleIcon
} from '@mui/icons-material';
import { styled } from '@mui/material/styles';
import { socialApi } from '../../services/socialApi';
import { useAuth } from '../../context/AuthContext';

// Styled components
const ConversationItem = styled(ListItem)(({ theme, selected }) => ({
  borderRadius: theme.shape.borderRadius,
  '&:hover': {
    backgroundColor: theme.palette.action.hover,
  },
  ...(selected && {
    backgroundColor: theme.palette.action.selected,
  }),
  cursor: 'pointer',
}));

// Mock data for conversations
const mockConversations = Array.from({ length: 10 }, (_, i) => ({
  id: `conversation-${i}`,
  participants: [
    {
      id: `user-${i}`,
      username: `user${i}`,
      displayName: `User ${i}`,
      profileImageUrl: `https://i.pravatar.cc/150?u=${i}`,
    },
    // Add more participants for group conversations
    ...(Math.random() > 0.7 ? [{
      id: `user-${i + 1}`,
      username: `user${i + 1}`,
      displayName: `User ${i + 1}`,
      profileImageUrl: `https://i.pravatar.cc/150?u=${i + 1}`,
    }] : []),
    ...(Math.random() > 0.9 ? [{
      id: `user-${i + 2}`,
      username: `user${i + 2}`,
      displayName: `User ${i + 2}`,
      profileImageUrl: `https://i.pravatar.cc/150?u=${i + 2}`,
    }] : [])
  ],
  lastMessage: {
    id: `message-${i}`,
    senderId: Math.random() > 0.5 ? `user-${i}` : 'current-user',
    content: `This is the last message in conversation ${i}. It could be longer or shorter.`,
    timestamp: new Date(Date.now() - i * 3600000).toISOString(),
  },
  unreadCount: Math.random() > 0.7 ? Math.floor(Math.random() * 10) + 1 : 0,
  isGroup: Math.random() > 0.7
}));

/**
 * Messages page component
 */
const Messages = () => {
  const navigate = useNavigate();
  const { user } = useAuth();
  const [loading, setLoading] = useState(true);
  const [conversations, setConversations] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  
  // Fetch conversations
  useEffect(() => {
    const fetchConversations = async () => {
      setLoading(true);
      try {
        // In a real app, we would fetch conversations from the API
        // const response = await socialApi.conversation.getConversations();
        // setConversations(response.data);
        
        // For now, use mock data
        setTimeout(() => {
          setConversations(mockConversations);
          setLoading(false);
        }, 1000);
      } catch (error) {
        console.error('Error fetching conversations:', error);
        setLoading(false);
      }
    };
    
    fetchConversations();
    
    // Subscribe to new messages
    socialApi.subscribeToMessages((message) => {
      // Update the conversations list with the new message
      setConversations(prevConversations => {
        const updatedConversations = [...prevConversations];
        const conversationIndex = updatedConversations.findIndex(c => c.id === message.conversationId);
        
        if (conversationIndex !== -1) {
          const conversation = { ...updatedConversations[conversationIndex] };
          conversation.lastMessage = message;
          
          // Increment unread count if the message is not from the current user
          if (message.senderId !== user?.id) {
            conversation.unreadCount = (conversation.unreadCount || 0) + 1;
          }
          
          // Move this conversation to the top
          updatedConversations.splice(conversationIndex, 1);
          updatedConversations.unshift(conversation);
        }
        
        return updatedConversations;
      });
    });
  }, [user?.id]);
  
  // Navigate to conversation detail
  const handleConversationClick = (conversationId) => {
    navigate(`/messages/${conversationId}`);
    
    // Mark conversation as read
    setConversations(prevConversations => 
      prevConversations.map(conv => 
        conv.id === conversationId
          ? { ...conv, unreadCount: 0 }
          : conv
      )
    );
  };
  
  // Create a new conversation
  const handleNewConversation = () => {
    navigate('/messages/new');
  };
  
  // Filter conversations based on search term
  const filteredConversations = conversations.filter(conversation => {
    const searchLower = searchTerm.toLowerCase();
    return conversation.participants.some(p => 
      p.displayName.toLowerCase().includes(searchLower) ||
      p.username.toLowerCase().includes(searchLower)
    ) || (
      conversation.lastMessage.content.toLowerCase().includes(searchLower)
    );
  });
  
  // Get conversation name and avatar
  const getConversationDetails = (conversation) => {
    // For a group conversation, join the names of participants
    if (conversation.isGroup || conversation.participants.length > 1) {
      const otherParticipants = conversation.participants.filter(p => p.id !== user?.id);
      const name = otherParticipants.map(p => p.displayName).join(', ');
      
      return {
        name: name || 'Group Conversation',
        avatar: null, // Use default group avatar
        isGroup: true
      };
    } else {
      // For a one-on-one conversation, use the other participant's info
      const otherParticipant = conversation.participants[0];
      
      return {
        name: otherParticipant.displayName,
        avatar: otherParticipant.profileImageUrl,
        isGroup: false
      };
    }
  };
  
  return (
    <Box sx={{ width: '100%' }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Typography variant="h5" component="h1">
          Messages
        </Typography>
        
        <Button
          variant="contained"
          startIcon={<EditIcon />}
          onClick={handleNewConversation}
        >
          New Message
        </Button>
      </Box>
      
      <Paper sx={{ mb: 3 }}>
        <Box sx={{ p: 2 }}>
          <TextField
            fullWidth
            placeholder="Search conversations..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <SearchIcon />
                </InputAdornment>
              ),
            }}
            size="small"
            variant="outlined"
          />
        </Box>
        
        <Divider />
        
        {loading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}>
            <CircularProgress />
          </Box>
        ) : filteredConversations.length === 0 ? (
          <Box sx={{ p: 3, textAlign: 'center' }}>
            <Typography color="textSecondary">
              {searchTerm ? 'No conversations match your search.' : 'No conversations yet.'}
            </Typography>
            <Button
              variant="contained"
              sx={{ mt: 2 }}
              onClick={handleNewConversation}
            >
              Start a conversation
            </Button>
          </Box>
        ) : (
          <List sx={{ width: '100%', bgcolor: 'background.paper' }}>
            {filteredConversations.map((conversation) => {
              const { name, avatar, isGroup } = getConversationDetails(conversation);
              const isFromCurrentUser = conversation.lastMessage.senderId === user?.id;
              
              return (
                <React.Fragment key={conversation.id}>
                  <ConversationItem
                    alignItems="flex-start"
                    onClick={() => handleConversationClick(conversation.id)}
                  >
                    <ListItemAvatar>
                      {conversation.unreadCount > 0 && (
                        <Badge
                          badgeContent={conversation.unreadCount}
                          color="primary"
                          overlap="circular"
                          anchorOrigin={{
                            vertical: 'top',
                            horizontal: 'right',
                          }}
                        >
                          {isGroup ? (
                            <Avatar>
                              {name.slice(0, 1).toUpperCase()}
                            </Avatar>
                          ) : (
                            <Avatar src={avatar} alt={name} />
                          )}
                        </Badge>
                      )}
                      
                      {conversation.unreadCount === 0 && (
                        isGroup ? (
                          <Avatar>
                            {name.slice(0, 1).toUpperCase()}
                          </Avatar>
                        ) : (
                          <Avatar src={avatar} alt={name} />
                        )
                      )}
                    </ListItemAvatar>
                    
                    <ListItemText
                      primary={
                        <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                          <Typography variant="subtitle1" component="span">
                            {name}
                          </Typography>
                          <Typography variant="body2" color="text.secondary" component="span">
                            {new Date(conversation.lastMessage.timestamp).toLocaleTimeString([], { 
                              hour: '2-digit', 
                              minute: '2-digit' 
                            })}
                          </Typography>
                        </Box>
                      }
                      secondary={
                        <Box sx={{ display: 'flex', alignItems: 'center' }}>
                          {isFromCurrentUser && (
                            <Typography variant="body2" color="text.secondary" component="span" sx={{ mr: 1 }}>
                              You:
                            </Typography>
                          )}
                          
                          <Typography
                            sx={{ display: 'inline', fontWeight: conversation.unreadCount > 0 ? 'bold' : 'normal' }}
                            component="span"
                            variant="body2"
                            color={conversation.unreadCount > 0 ? "text.primary" : "text.secondary"}
                          >
                            {conversation.lastMessage.content.length > 60
                              ? `${conversation.lastMessage.content.substring(0, 60)}...`
                              : conversation.lastMessage.content
                            }
                          </Typography>
                          
                          {conversation.unreadCount > 0 && (
                            <CircleIcon 
                              color="primary" 
                              sx={{ fontSize: 8, ml: 1 }} 
                            />
                          )}
                        </Box>
                      }
                    />
                  </ConversationItem>
                  <Divider variant="inset" component="li" />
                </React.Fragment>
              );
            })}
          </List>
        )}
      </Paper>
    </Box>
  );
};

export default Messages;
