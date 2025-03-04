import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import {
  Box,
  Tabs,
  Tab,
  Typography,
  Paper,
  List,
  ListItem,
  ListItemAvatar,
  ListItemText,
  Avatar,
  Button,
  Divider,
  CircularProgress,
  Card,
  CardContent,
  CardActions,
  IconButton,
  TextField,
  InputAdornment
} from '@mui/material';
import {
  Search as SearchIcon,
  Favorite as FavoriteIcon,
  Comment as CommentIcon,
  Share as ShareIcon,
  PersonAdd as PersonAddIcon,
  Check as CheckIcon
} from '@mui/icons-material';
import { socialApi } from '../../services/socialApi';
import { useAuth } from '../../context/AuthContext';

// Mock data for initial development
const mockActivities = Array.from({ length: 10 }, (_, i) => ({
  id: `activity-${i}`,
  userId: `user-${i % 5}`,
  username: `user${i % 5}`,
  displayName: `User ${i % 5}`,
  profileImageUrl: `https://i.pravatar.cc/150?u=${i % 5}`,
  type: ['post', 'project', 'follow', 'comment'][i % 4],
  content: `This is activity ${i} content. It could be a post, project update, follow action, or a comment.`,
  targetId: i % 2 === 0 ? `project-${i}` : `post-${i}`,
  targetName: i % 2 === 0 ? `Project ${i}` : `Post ${i}`,
  createdAt: new Date(Date.now() - i * 3600000).toISOString(),
  likes: Math.floor(Math.random() * 50),
  comments: Math.floor(Math.random() * 20),
  hasLiked: Math.random() > 0.5
}));

const mockUsers = Array.from({ length: 20 }, (_, i) => ({
  id: `user-${i}`,
  username: `user${i}`,
  displayName: `User ${i}`,
  profileImageUrl: `https://i.pravatar.cc/150?u=${i}`,
  bio: `This is the bio for User ${i}. They are interested in technology, AI, and social networking.`,
  isFollowing: Math.random() > 0.7
}));

// Tab panel component
function TabPanel(props) {
  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`social-tabpanel-${index}`}
      aria-labelledby={`social-tab-${index}`}
      {...other}
    >
      {value === index && (
        <Box sx={{ p: 3 }}>
          {children}
        </Box>
      )}
    </div>
  );
}

// Activity item component
const ActivityItem = ({ activity }) => {
  const [liked, setLiked] = useState(activity.hasLiked);
  const [likeCount, setLikeCount] = useState(activity.likes);
  
  const handleLike = () => {
    setLiked(!liked);
    setLikeCount(prev => liked ? prev - 1 : prev + 1);
    
    // In a real app, we would call an API to like/unlike the activity
    // if (!liked) {
    //   socialApi.activity.likeActivity(activity.id);
    // } else {
    //   socialApi.activity.unlikeActivity(activity.id);
    // }
  };
  
  const getActivityContent = () => {
    switch (activity.type) {
      case 'post':
        return (
          <Typography variant="body2" color="text.secondary">
            {activity.content}
          </Typography>
        );
      case 'project':
        return (
          <Typography variant="body2" color="text.secondary">
            Created a new project: <b>{activity.targetName}</b>
            <br />
            {activity.content}
          </Typography>
        );
      case 'follow':
        return (
          <Typography variant="body2" color="text.secondary">
            Started following <b>{activity.targetName}</b>
          </Typography>
        );
      case 'comment':
        return (
          <Typography variant="body2" color="text.secondary">
            Commented on <b>{activity.targetName}</b>: {activity.content}
          </Typography>
        );
      default:
        return (
          <Typography variant="body2" color="text.secondary">
            {activity.content}
          </Typography>
        );
    }
  };
  
  return (
    <Card sx={{ mb: 2 }}>
      <CardContent>
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
          <ListItemAvatar>
            <Avatar 
              src={activity.profileImageUrl} 
              alt={activity.displayName}
              component={Link}
              to={`/profile/${activity.userId}`}
              sx={{ cursor: 'pointer' }}
            />
          </ListItemAvatar>
          <Box>
            <Typography 
              variant="subtitle1" 
              component={Link} 
              to={`/profile/${activity.userId}`}
              sx={{ textDecoration: 'none', color: 'inherit', fontWeight: 'bold' }}
            >
              {activity.displayName}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              @{activity.username} • {new Date(activity.createdAt).toLocaleString()}
            </Typography>
          </Box>
        </Box>
        
        {getActivityContent()}
      </CardContent>
      
      <CardActions disableSpacing>
        <IconButton aria-label="like" onClick={handleLike} color={liked ? 'primary' : 'default'}>
          <FavoriteIcon />
        </IconButton>
        <Typography variant="body2" color="text.secondary" sx={{ mr: 2 }}>
          {likeCount}
        </Typography>
        
        <IconButton aria-label="comment" component={Link} to={`/post/${activity.id}`}>
          <CommentIcon />
        </IconButton>
        <Typography variant="body2" color="text.secondary" sx={{ mr: 2 }}>
          {activity.comments}
        </Typography>
        
        <IconButton aria-label="share">
          <ShareIcon />
        </IconButton>
      </CardActions>
    </Card>
  );
};

// User item component for followers/following tabs
const UserItem = ({ user, onFollowToggle }) => {
  const [isFollowing, setIsFollowing] = useState(user.isFollowing);
  
  const handleFollowToggle = async () => {
    setIsFollowing(!isFollowing);
    onFollowToggle(user.id, !isFollowing);
  };
  
  return (
    <ListItem
      secondaryAction={
        <Button
          variant={isFollowing ? 'outlined' : 'contained'}
          startIcon={isFollowing ? <CheckIcon /> : <PersonAddIcon />}
          onClick={handleFollowToggle}
          size="small"
        >
          {isFollowing ? 'Following' : 'Follow'}
        </Button>
      }
    >
      <ListItemAvatar>
        <Avatar 
          src={user.profileImageUrl} 
          alt={user.displayName}
          component={Link}
          to={`/profile/${user.id}`}
        />
      </ListItemAvatar>
      <ListItemText
        primary={
          <Typography 
            component={Link} 
            to={`/profile/${user.id}`}
            sx={{ textDecoration: 'none', color: 'inherit', fontWeight: 'bold' }}
          >
            {user.displayName}
          </Typography>
        }
        secondary={
          <>
            <Typography component="span" variant="body2" color="text.secondary">
              @{user.username}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              {user.bio}
            </Typography>
          </>
        }
      />
    </ListItem>
  );
};

// Main Social component
const Social = () => {
  const { user } = useAuth();
  const [tabValue, setTabValue] = useState(0);
  const [loading, setLoading] = useState(true);
  const [activities, setActivities] = useState([]);
  const [followers, setFollowers] = useState([]);
  const [following, setFollowing] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  
  // Handle tab change
  const handleTabChange = (event, newValue) => {
    setTabValue(newValue);
  };
  
  // Handle follow/unfollow
  const handleFollowToggle = async (userId, shouldFollow) => {
    try {
      if (shouldFollow) {
        await socialApi.follow.followUser(userId);
      } else {
        await socialApi.follow.unfollowUser(userId);
      }
    } catch (error) {
      console.error('Error toggling follow status:', error);
      // If API call fails, revert the UI change
      if (tabValue === 1) { // Followers tab
        setFollowers(prev => 
          prev.map(follower => 
            follower.id === userId 
              ? { ...follower, isFollowing: !shouldFollow }
              : follower
          )
        );
      } else if (tabValue === 2) { // Following tab
        setFollowing(prev => 
          prev.map(followedUser => 
            followedUser.id === userId 
              ? { ...followedUser, isFollowing: !shouldFollow }
              : followedUser
          )
        );
      }
    }
  };
  
  // Load data based on active tab
  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);
      
      try {
        switch (tabValue) {
          case 0: // Activity feed
            // For now, use mock data
            // In a real app: const response = await socialApi.activity.getFollowingActivities();
            setTimeout(() => {
              setActivities(mockActivities);
              setLoading(false);
            }, 1000);
            break;
            
          case 1: // Followers
            // For now, use mock data
            // In a real app: const response = await socialApi.follow.getFollowers(user.id);
            setTimeout(() => {
              setFollowers(mockUsers.slice(0, 10));
              setLoading(false);
            }, 1000);
            break;
            
          case 2: // Following
            // For now, use mock data
            // In a real app: const response = await socialApi.follow.getFollowing(user.id);
            setTimeout(() => {
              setFollowing(mockUsers.slice(5, 15));
              setLoading(false);
            }, 1000);
            break;
            
          default:
            setLoading(false);
            break;
        }
      } catch (error) {
        console.error('Error fetching data:', error);
        setLoading(false);
      }
    };
    
    fetchData();
  }, [tabValue, user?.id]);
  
  // Filter users based on search term
  const filteredFollowers = followers.filter(follower => 
    follower.displayName.toLowerCase().includes(searchTerm.toLowerCase()) ||
    follower.username.toLowerCase().includes(searchTerm.toLowerCase())
  );
  
  const filteredFollowing = following.filter(followedUser => 
    followedUser.displayName.toLowerCase().includes(searchTerm.toLowerCase()) ||
    followedUser.username.toLowerCase().includes(searchTerm.toLowerCase())
  );
  
  return (
    <Box sx={{ width: '100%' }}>
      <Paper sx={{ width: '100%', mb: 2 }}>
        <Tabs
          value={tabValue}
          onChange={handleTabChange}
          indicatorColor="primary"
          textColor="primary"
          variant="fullWidth"
        >
          <Tab label="Activity Feed" />
          <Tab label="Followers" />
          <Tab label="Following" />
        </Tabs>
        
        {/* Search input for followers/following tabs */}
        {(tabValue === 1 || tabValue === 2) && (
          <Box sx={{ p: 2 }}>
            <TextField
              fullWidth
              placeholder="Search users..."
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
        )}
      </Paper>
      
      {/* Loading indicator */}
      {loading && (
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
          <CircularProgress />
        </Box>
      )}
      
      {/* Activity feed tab */}
      <TabPanel value={tabValue} index={0}>
        {!loading && activities.length === 0 && (
          <Typography align="center" color="textSecondary">
            No activities found. Follow users to see their activities.
          </Typography>
        )}
        
        {!loading && activities.map((activity) => (
          <ActivityItem key={activity.id} activity={activity} />
        ))}
      </TabPanel>
      
      {/* Followers tab */}
      <TabPanel value={tabValue} index={1}>
        {!loading && filteredFollowers.length === 0 && (
          <Typography align="center" color="textSecondary">
            {searchTerm ? 'No matches found.' : 'No followers yet.'}
          </Typography>
        )}
        
        {!loading && (
          <List>
            {filteredFollowers.map((follower) => (
              <React.Fragment key={follower.id}>
                <UserItem 
                  user={follower} 
                  onFollowToggle={handleFollowToggle}
                />
                <Divider variant="inset" component="li" />
              </React.Fragment>
            ))}
          </List>
        )}
      </TabPanel>
      
      {/* Following tab */}
      <TabPanel value={tabValue} index={2}>
        {!loading && filteredFollowing.length === 0 && (
          <Typography align="center" color="textSecondary">
            {searchTerm ? 'No matches found.' : 'Not following anyone yet.'}
          </Typography>
        )}
        
        {!loading && (
          <List>
            {filteredFollowing.map((followedUser) => (
              <React.Fragment key={followedUser.id}>
                <UserItem 
                  user={followedUser} 
                  onFollowToggle={handleFollowToggle}
                />
                <Divider variant="inset" component="li" />
              </React.Fragment>
            ))}
          </List>
        )}
      </TabPanel>
    </Box>
  );
};

export default Social;
