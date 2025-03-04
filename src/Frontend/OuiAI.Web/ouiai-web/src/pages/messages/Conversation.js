import React, { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Typography,
  Paper,
  TextField,
  IconButton,
  Avatar,
  Divider,
  CircularProgress,
  AppBar,
  Toolbar,
  List,
  ListItem,
  ListItemText,
  ListItemAvatar,
  Tooltip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button
} from '@mui/material';
import {
  Send as SendIcon,
  ArrowBack as ArrowBackIcon,
  MoreVert as MoreVertIcon,
  Info as InfoIcon,
  Delete as DeleteIcon
} from '@mui/icons-material';
import { styled } from '@mui/material/styles';
import { socialApi } from '../../services/socialApi';
import { useAuth } from '../../context/AuthContext';

// Styled components
const MessageBubble = styled(Box, {
  shouldForwardProp: (prop) => prop !== 'isSentByCurrentUser'
})(({ theme, isSentByCurrentUser }) => ({
  maxWidth: '70%',
  padding: theme.spacing(1, 2),
  borderRadius: 16,
  marginBottom: theme.spacing(1),
  wordBreak: 'break-word',
  ...(isSentByCurrentUser
    ? {
        backgroundColor: theme.palette.primary.main,
        color: theme.palette.primary.contrastText,
        alignSelf: 'flex-end',
        borderBottomRightRadius: 4,
      }
    : {
        backgroundColor: theme.palette.grey[100],
        color: theme.palette.text.primary,
        alignSelf: 'flex-start',
        borderBottomLeftRadius: 4,
      })
}));

const MessageTimestamp = styled(Typography)(({ theme, isSentByCurrentUser }) => ({
  fontSize: '0.75rem',
  color: isSentByCurrentUser ? theme.palette.primary.contrastText : theme.palette.text.secondary,
  opacity: 0.7,
  marginTop: theme.spacing(0.5),
  textAlign: isSentByCurrentUser ? 'right' : 'left'
}));

const MessagesContainer = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  padding: theme.spacing(2),
  overflowY: 'auto',
  flexGrow: 1
}));

// Generate mock messages for development
const generateMockMessages = (conversationId) => {
  const count = 20 + Math.floor(Math.random() * 30);
  return Array.from({ length: count }, (_, i) => ({
    id: `message-${conversationId}-${i}`,
    conversationId,
    senderId: Math.random() > 0.5 ? `user-${i % 5}` : 'current-user',
    content: `This is message ${i} in the conversation. It could be short or it could be a really long message that spans multiple lines to test the layout and styling of the bubbles.`.substring(0, Math.random() * 150 + 20),
    timestamp: new Date(Date.now() - (count - i) * 900000).toISOString(),
    status: ['sent', 'delivered', 'read'][Math.floor(Math.random() * 3)]
  }));
};

// Generate mock conversation for development
const generateMockConversation = (conversationId) => {
  const isGroup = Math.random() > 0.7;
  const participantCount = isGroup ? Math.floor(Math.random() * 5) + 2 : 1;
  
  return {
    id: conversationId,
    participants: Array.from({ length: participantCount }, (_, i) => ({
      id: `user-${i}`,
      username: `user${i}`,
      displayName: `User ${i}`,
      profileImageUrl: `https://i.pravatar.cc/150?u=${i}`,
    })),
    isGroup,
    title: isGroup ? 'Group Conversation' : undefined,
    createdAt: new Date(Date.now() - 7 * 24 * 3600000).toISOString(),
  };
};

/**
 * Conversation page component
 */
const Conversation = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user } = useAuth();
  const [loading, setLoading] = useState(true);
  const [sendingMessage, setSendingMessage] = useState(false);
  const [conversation, setConversation] = useState(null);
  const [messages, setMessages] = useState([]);
  const [messageText, setMessageText] = useState('');
  const [infoDialogOpen, setInfoDialogOpen] = useState(false);
  const messagesEndRef = useRef(null);
  const messagesContainerRef = useRef(null);
  
  // Scroll to bottom when messages change
  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };
  
  // Fetch conversation and messages
  useEffect(() => {
    const fetchConversationData = async () => {
      setLoading(true);
      try {
        // In a real app, we would fetch data from the API
        // const conversationResponse = await socialApi.conversation.getConversation(id);
        // const messagesResponse = await socialApi.message.getMessages(id);
        // setConversation(conversationResponse.data);
        // setMessages(messagesResponse.data);
        
        // For now, use mock data
        setTimeout(() => {
          setConversation(generateMockConversation(id));
          setMessages(generateMockMessages(id));
          setLoading(false);
        }, 1000);
      } catch (error) {
        console.error('Error fetching conversation data:', error);
        setLoading(false);
      }
    };
    
    if (id) {
      fetchConversationData();
    }
    
    // Subscribe to new messages for this conversation
    const unsubscribe = socialApi.subscribeToMessages((message) => {
      if (message.conversationId === id) {
        setMessages(prevMessages => [...prevMessages, message]);
      }
    });
    
    return () => {
      unsubscribe && unsubscribe();
    };
  }, [id]);
  
  // Scroll to bottom when messages are loaded or new messages arrive
  useEffect(() => {
    if (!loading) {
      scrollToBottom();
    }
  }, [messages, loading]);
  
  // Handle message send
  const handleSendMessage = async () => {
    if (!messageText.trim()) return;
    
    const newMessage = {
      id: `message-${Date.now()}`,
      conversationId: id,
      senderId: 'current-user',
      content: messageText,
      timestamp: new Date().toISOString(),
      status: 'sent'
    };
    
    setMessages(prevMessages => [...prevMessages, newMessage]);
    setMessageText('');
    setSendingMessage(true);
    
    try {
      // In a real app, we would send the message via the API
      // await socialApi.message.sendMessage(id, messageText);
      
      // Simulate API delay
      setTimeout(() => {
        // Update message status to 'delivered'
        setMessages(prevMessages => 
          prevMessages.map(msg => 
            msg.id === newMessage.id
              ? { ...msg, status: 'delivered' }
              : msg
          )
        );
        setSendingMessage(false);
      }, 1000);
    } catch (error) {
      console.error('Error sending message:', error);
      setSendingMessage(false);
      
      // Show error status on the message
      setMessages(prevMessages => 
        prevMessages.map(msg => 
          msg.id === newMessage.id
            ? { ...msg, status: 'error' }
            : msg
        )
      );
    }
  };
  
  // Get conversation title and participants info
  const getConversationDetails = () => {
    if (!conversation) return { title: '', participants: [] };
    
    if (conversation.isGroup) {
      return {
        title: conversation.title || 'Group Conversation',
        participants: conversation.participants
      };
    } else {
      const otherParticipant = conversation.participants[0];
      return {
        title: otherParticipant.displayName,
        participants: conversation.participants
      };
    }
  };
  
  const { title, participants } = getConversationDetails();
  
  // Format message timestamp
  const formatMessageTime = (timestamp) => {
    const date = new Date(timestamp);
    return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  };
  
  // Format message date for date separators
  const formatMessageDate = (timestamp) => {
    const date = new Date(timestamp);
    const today = new Date();
    const yesterday = new Date(today);
    yesterday.setDate(yesterday.getDate() - 1);
    
    if (date.toDateString() === today.toDateString()) {
      return 'Today';
    } else if (date.toDateString() === yesterday.toDateString()) {
      return 'Yesterday';
    } else {
      return date.toLocaleDateString();
    }
  };
  
  // Group messages by date
  const messagesByDate = messages.reduce((groups, message) => {
    const date = new Date(message.timestamp).toDateString();
    if (!groups[date]) {
      groups[date] = [];
    }
    groups[date].push(message);
    return groups;
  }, {});
  
  // Navigate back
  const handleBack = () => {
    navigate('/messages');
  };
  
  // Open info dialog
  const handleInfoClick = () => {
    setInfoDialogOpen(true);
  };
  
  return (
    <Box sx={{ height: 'calc(100vh - 170px)', display: 'flex', flexDirection: 'column' }}>
      {/* Conversation header */}
      <AppBar position="static" color="default" elevation={0}>
        <Toolbar>
          <IconButton edge="start" color="inherit" onClick={handleBack} sx={{ mr: 1 }}>
            <ArrowBackIcon />
          </IconButton>
          
          {loading ? (
            <CircularProgress size={24} sx={{ mr: 2 }} />
          ) : (
            <>
              <Avatar
                src={conversation?.isGroup ? null : participants[0]?.profileImageUrl}
                alt={title}
                sx={{ mr: 2 }}
              >
                {conversation?.isGroup && title.charAt(0)}
              </Avatar>
              
              <Typography variant="h6" noWrap component="div" sx={{ flexGrow: 1 }}>
                {title}
              </Typography>
              
              <Tooltip title="Conversation Info">
                <IconButton color="inherit" onClick={handleInfoClick}>
                  <InfoIcon />
                </IconButton>
              </Tooltip>
              
              <IconButton edge="end" color="inherit">
                <MoreVertIcon />
              </IconButton>
            </>
          )}
        </Toolbar>
      </AppBar>
      
      {/* Messages area */}
      <Paper 
        elevation={0} 
        sx={{ 
          flexGrow: 1, 
          display: 'flex', 
          flexDirection: 'column',
          overflow: 'hidden'
        }}
      >
        {loading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%' }}>
            <CircularProgress />
          </Box>
        ) : (
          <MessagesContainer ref={messagesContainerRef}>
            {Object.entries(messagesByDate).map(([date, dateMessages]) => (
              <Box key={date} sx={{ width: '100%' }}>
                <Box sx={{ 
                  display: 'flex', 
                  justifyContent: 'center', 
                  my: 2 
                }}>
                  <Typography 
                    variant="caption" 
                    sx={{ 
                      backgroundColor: 'rgba(0, 0, 0, 0.08)', 
                      px: 2, 
                      py: 0.5, 
                      borderRadius: 16 
                    }}
                  >
                    {formatMessageDate(dateMessages[0].timestamp)}
                  </Typography>
                </Box>
                
                {dateMessages.map((message, index) => {
                  const isSentByCurrentUser = message.senderId === 'current-user';
                  
                  return (
                    <Box 
                      key={message.id} 
                      sx={{ 
                        display: 'flex', 
                        flexDirection: 'column',
                        alignItems: isSentByCurrentUser ? 'flex-end' : 'flex-start'
                      }}
                    >
                      <MessageBubble isSentByCurrentUser={isSentByCurrentUser}>
                        {message.content}
                      </MessageBubble>
                      
                      <MessageTimestamp 
                        variant="caption" 
                        isSentByCurrentUser={isSentByCurrentUser}
                      >
                        {formatMessageTime(message.timestamp)}
                        {isSentByCurrentUser && (
                          <Typography component="span" variant="caption" sx={{ ml: 0.5 }}>
                            {message.status === 'read' ? '✓✓' : message.status === 'delivered' ? '✓✓' : '✓'}
                          </Typography>
                        )}
                      </MessageTimestamp>
                    </Box>
                  );
                })}
              </Box>
            ))}
            
            <div ref={messagesEndRef} />
          </MessagesContainer>
        )}
        
        <Divider />
        
        {/* Message input */}
        <Box sx={{ p: 2, display: 'flex', alignItems: 'center' }}>
          <TextField
            fullWidth
            placeholder="Type a message..."
            variant="outlined"
            size="small"
            value={messageText}
            onChange={(e) => setMessageText(e.target.value)}
            onKeyPress={(e) => {
              if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                handleSendMessage();
              }
            }}
            disabled={loading || sendingMessage}
            sx={{ mr: 1 }}
            multiline
            maxRows={4}
          />
          
          <IconButton 
            color="primary" 
            onClick={handleSendMessage} 
            disabled={!messageText.trim() || loading || sendingMessage}
          >
            <SendIcon />
          </IconButton>
        </Box>
      </Paper>
      
      {/* Conversation info dialog */}
      <Dialog open={infoDialogOpen} onClose={() => setInfoDialogOpen(false)}>
        <DialogTitle>
          {conversation?.isGroup ? 'Group Info' : 'Conversation Info'}
        </DialogTitle>
        
        <DialogContent>
          <Typography variant="subtitle1" sx={{ fontWeight: 'bold', mb: 2 }}>
            Participants
          </Typography>
          
          <List>
            {participants.map((participant) => (
              <ListItem key={participant.id}>
                <ListItemAvatar>
                  <Avatar src={participant.profileImageUrl} alt={participant.displayName} />
                </ListItemAvatar>
                <ListItemText
                  primary={participant.displayName}
                  secondary={`@${participant.username}`}
                />
              </ListItem>
            ))}
          </List>
          
          <Divider sx={{ my: 2 }} />
          
          <Typography variant="body2" color="textSecondary">
            Created: {conversation && new Date(conversation.createdAt).toLocaleDateString()}
          </Typography>
        </DialogContent>
        
        <DialogActions>
          <Button 
            color="error"
            startIcon={<DeleteIcon />}
          >
            Delete Conversation
          </Button>
          <Button onClick={() => setInfoDialogOpen(false)}>
            Close
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default Conversation;
