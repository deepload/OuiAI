import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Typography,
  Paper,
  Button,
  Avatar,
  Tabs,
  Tab,
  Divider,
  TextField,
  Grid,
  Card,
  CardContent,
  CardActions,
  IconButton,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Badge
} from '@mui/material';
import {
  Edit as EditIcon,
  Message as MessageIcon,
  PersonAdd as PersonAddIcon,
  PersonRemove as PersonRemoveIcon,
  Favorite as FavoriteIcon,
  Comment as CommentIcon,
  Share as ShareIcon,
  Check as CheckIcon,
  PhotoCamera as PhotoCameraIcon
} from '@mui/icons-material';
import { styled } from '@mui/material/styles';
import { socialApi } from '../../services/socialApi';
import { useAuth } from '../../context/AuthContext';

// Styled components
const ProfileCover = styled(Box)(({ theme }) => ({
  height: 200,
  backgroundColor: theme.palette.primary.light,
  position: 'relative',
  borderRadius: `${theme.shape.borderRadius}px ${theme.shape.borderRadius}px 0 0`,
  backgroundSize: 'cover',
  backgroundPosition: 'center',
}));

const ProfileAvatar = styled(Avatar)(({ theme }) => ({
  width: 150,
  height: 150,
  border: `4px solid ${theme.palette.background.paper}`,
  position: 'absolute',
  bottom: -75,
  left: theme.spacing(3),
}));

// Tab panel component
function TabPanel(props) {
  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`profile-tabpanel-${index}`}
      aria-labelledby={`profile-tab-${index}`}
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

// Mock user profile data
const mockUserProfile = {
  id: 'user-1',
  username: 'johndoe',
  displayName: 'John Doe',
  bio: 'Software engineer passionate about web development, AI, and building awesome user experiences. Open to collaborations and new projects.',
  location: 'San Francisco, CA',
  website: 'https://johndoe.dev',
  profileImageUrl: 'https://i.pravatar.cc/300?u=1',
  coverImageUrl: 'https://source.unsplash.com/random/1200x400/?abstract',
  followers: 235,
  following: 128,
  joinedDate: '2022-06-15T00:00:00Z',
  isFollowing: false,
  isCurrentUser: false,
};

// Mock activities data
const mockActivities = Array.from({ length: 10 }, (_, i) => ({
  id: `activity-${i}`,
  userId: 'user-1',
  username: 'johndoe',
  displayName: 'John Doe',
  profileImageUrl: 'https://i.pravatar.cc/150?u=1',
  type: ['post', 'project', 'follow', 'comment'][i % 4],
  content: `This is activity ${i} content. It could be a post, project update, follow action, or a comment.`,
  targetId: i % 2 === 0 ? `project-${i}` : `post-${i}`,
  targetName: i % 2 === 0 ? `Project ${i}` : `Post ${i}`,
  createdAt: new Date(Date.now() - i * 3600000).toISOString(),
  likes: Math.floor(Math.random() * 50),
  comments: Math.floor(Math.random() * 20),
  hasLiked: Math.random() > 0.5
}));

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
        {getActivityContent()}
      </CardContent>
      
      <CardActions disableSpacing>
        <IconButton aria-label="like" onClick={handleLike} color={liked ? 'primary' : 'default'}>
          <FavoriteIcon />
        </IconButton>
        <Typography variant="body2" color="text.secondary" sx={{ mr: 2 }}>
          {likeCount}
        </Typography>
        
        <IconButton aria-label="comment">
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

/**
 * Profile page component
 */
const Profile = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user } = useAuth();
  const [profile, setProfile] = useState(null);
  const [activities, setActivities] = useState([]);
  const [loading, setLoading] = useState(true);
  const [tabValue, setTabValue] = useState(0);
  const [isEditing, setIsEditing] = useState(false);
  const [editForm, setEditForm] = useState({
    displayName: '',
    bio: '',
    location: '',
    website: ''
  });
  const [isFollowing, setIsFollowing] = useState(false);
  const [followLoading, setFollowLoading] = useState(false);
  const [avatarDialogOpen, setAvatarDialogOpen] = useState(false);
  
  // Fetch profile data
  useEffect(() => {
    const fetchProfileData = async () => {
      setLoading(true);
      
      try {
        // In a real app, we would fetch profile from the API
        // const profileResponse = id 
        //   ? await socialApi.user.getUserProfile(id)
        //   : await socialApi.user.getCurrentUserProfile();
        // setProfile(profileResponse.data);
        
        // For now, use mock data
        setTimeout(() => {
          const mockProfile = {...mockUserProfile};
          
          // If no ID is provided, it's the current user's profile
          if (!id) {
            mockProfile.isCurrentUser = true;
            mockProfile.displayName = user?.displayName || 'Current User';
            mockProfile.username = user?.username || 'currentuser';
            mockProfile.profileImageUrl = user?.profileImageUrl;
          } else {
            mockProfile.isCurrentUser = id === user?.id;
          }
          
          setProfile(mockProfile);
          setIsFollowing(mockProfile.isFollowing);
          setEditForm({
            displayName: mockProfile.displayName,
            bio: mockProfile.bio || '',
            location: mockProfile.location || '',
            website: mockProfile.website || ''
          });
          
          // Also fetch activities
          setActivities(mockActivities);
          setLoading(false);
        }, 1000);
      } catch (error) {
        console.error('Error fetching profile data:', error);
        setLoading(false);
      }
    };
    
    fetchProfileData();
  }, [id, user]);
  
  // Handle tab change
  const handleTabChange = (event, newValue) => {
    setTabValue(newValue);
  };
  
  // Handle edit form changes
  const handleEditFormChange = (e) => {
    const { name, value } = e.target;
    setEditForm({
      ...editForm,
      [name]: value
    });
  };
  
  // Handle save profile
  const handleSaveProfile = async () => {
    try {
      // In a real app, we would call the API to update the profile
      // await socialApi.user.updateProfile(editForm);
      
      // Update the local profile state
      setProfile({
        ...profile,
        displayName: editForm.displayName,
        bio: editForm.bio,
        location: editForm.location,
        website: editForm.website
      });
      
      setIsEditing(false);
    } catch (error) {
      console.error('Error updating profile:', error);
    }
  };
  
  // Handle follow/unfollow
  const handleFollowToggle = async () => {
    setFollowLoading(true);
    
    try {
      if (isFollowing) {
        // In a real app, we would call the API to unfollow
        // await socialApi.follow.unfollowUser(profile.id);
      } else {
        // In a real app, we would call the API to follow
        // await socialApi.follow.followUser(profile.id);
      }
      
      setIsFollowing(!isFollowing);
    } catch (error) {
      console.error('Error toggling follow status:', error);
    } finally {
      setFollowLoading(false);
    }
  };
  
  // Handle message
  const handleMessage = () => {
    navigate(`/messages/new?userId=${profile.id}`);
  };
  
  // Handle avatar upload
  const handleAvatarUpload = (event) => {
    const file = event.target.files[0];
    if (file) {
      // In a real app, we would upload the file to the server
      // socialApi.user.updateProfileImage(file)
      //   .then(response => {
      //     setProfile({
      //       ...profile,
      //       profileImageUrl: response.data.imageUrl
      //     });
      //     setAvatarDialogOpen(false);
      //   })
      //   .catch(error => {
      //     console.error('Error uploading avatar:', error);
      //   });
      
      // For now, just simulate a successful upload
      const reader = new FileReader();
      reader.onload = (e) => {
        setProfile({
          ...profile,
          profileImageUrl: e.target.result
        });
        setAvatarDialogOpen(false);
      };
      reader.readAsDataURL(file);
    }
  };
  
  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '50vh' }}>
        <CircularProgress />
      </Box>
    );
  }
  
  return (
    <Box sx={{ mb: 4 }}>
      <Paper sx={{ borderRadius: 2, overflow: 'hidden', mb: 2 }}>
        {/* Profile header */}
        <ProfileCover 
          sx={{ 
            backgroundImage: `url(${profile.coverImageUrl || ''})` 
          }}
        />
        
        <Box sx={{ height: 110, position: 'relative' }}>
          <Badge
            overlap="circular"
            anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
            badgeContent={
              profile.isCurrentUser && (
                <IconButton
                  sx={{ bgcolor: 'primary.main', color: 'white', '&:hover': { bgcolor: 'primary.dark' } }}
                  onClick={() => setAvatarDialogOpen(true)}
                >
                  <PhotoCameraIcon fontSize="small" />
                </IconButton>
              )
            }
          >
            <ProfileAvatar src={profile.profileImageUrl} alt={profile.displayName} />
          </Badge>
        </Box>
        
        <Box sx={{ p: 3, pt: 0 }}>
          <Box sx={{ display: 'flex', justifyContent: 'flex-end', mb: 2 }}>
            {profile.isCurrentUser ? (
              <Button
                variant="outlined"
                startIcon={<EditIcon />}
                onClick={() => setIsEditing(!isEditing)}
              >
                {isEditing ? 'Cancel' : 'Edit Profile'}
              </Button>
            ) : (
              <Box>
                <Button
                  variant={isFollowing ? 'outlined' : 'contained'}
                  startIcon={isFollowing ? <CheckIcon /> : <PersonAddIcon />}
                  onClick={handleFollowToggle}
                  disabled={followLoading}
                  sx={{ mr: 1 }}
                >
                  {followLoading ? 'Processing...' : (isFollowing ? 'Following' : 'Follow')}
                </Button>
                
                <Button
                  variant="outlined"
                  startIcon={<MessageIcon />}
                  onClick={handleMessage}
                >
                  Message
                </Button>
              </Box>
            )}
          </Box>
          
          {isEditing ? (
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <TextField
                  fullWidth
                  label="Display Name"
                  name="displayName"
                  value={editForm.displayName}
                  onChange={handleEditFormChange}
                  variant="outlined"
                  margin="normal"
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  fullWidth
                  label="Bio"
                  name="bio"
                  value={editForm.bio}
                  onChange={handleEditFormChange}
                  variant="outlined"
                  multiline
                  rows={3}
                  margin="normal"
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Location"
                  name="location"
                  value={editForm.location}
                  onChange={handleEditFormChange}
                  variant="outlined"
                  margin="normal"
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Website"
                  name="website"
                  value={editForm.website}
                  onChange={handleEditFormChange}
                  variant="outlined"
                  margin="normal"
                />
              </Grid>
              <Grid item xs={12}>
                <Button
                  variant="contained"
                  color="primary"
                  onClick={handleSaveProfile}
                  sx={{ mt: 2 }}
                >
                  Save Changes
                </Button>
              </Grid>
            </Grid>
          ) : (
            <>
              <Typography variant="h4" sx={{ fontWeight: 'bold', mb: 0.5 }}>
                {profile.displayName}
              </Typography>
              
              <Typography variant="body2" color="textSecondary" sx={{ mb: 1 }}>
                @{profile.username}
              </Typography>
              
              {profile.bio && (
                <Typography variant="body1" sx={{ my: 2 }}>
                  {profile.bio}
                </Typography>
              )}
              
              <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 3, my: 2 }}>
                {profile.location && (
                  <Typography variant="body2" color="textSecondary">
                    📍 {profile.location}
                  </Typography>
                )}
                
                {profile.website && (
                  <Typography variant="body2" color="textSecondary">
                    🔗 <a href={profile.website} target="_blank" rel="noopener noreferrer">{profile.website}</a>
                  </Typography>
                )}
                
                <Typography variant="body2" color="textSecondary">
                  📅 Joined {new Date(profile.joinedDate).toLocaleDateString()}
                </Typography>
              </Box>
              
              <Box sx={{ display: 'flex', gap: 4, my: 2 }}>
                <Typography variant="body2">
                  <strong>{profile.followers}</strong> Followers
                </Typography>
                
                <Typography variant="body2">
                  <strong>{profile.following}</strong> Following
                </Typography>
              </Box>
            </>
          )}
        </Box>
        
        {/* Tabs navigation */}
        <Tabs
          value={tabValue}
          onChange={handleTabChange}
          indicatorColor="primary"
          textColor="primary"
          variant="fullWidth"
        >
          <Tab label="Activities" />
          <Tab label="Likes" />
          <Tab label="Projects" />
        </Tabs>
      </Paper>
      
      {/* Tab panels */}
      <TabPanel value={tabValue} index={0}>
        {activities.length === 0 ? (
          <Typography align="center" color="textSecondary">
            No activities yet.
          </Typography>
        ) : (
          activities.map((activity) => (
            <ActivityItem key={activity.id} activity={activity} />
          ))
        )}
      </TabPanel>
      
      <TabPanel value={tabValue} index={1}>
        <Typography align="center" color="textSecondary">
          No liked items yet.
        </Typography>
      </TabPanel>
      
      <TabPanel value={tabValue} index={2}>
        <Typography align="center" color="textSecondary">
          No projects yet.
        </Typography>
      </TabPanel>
      
      {/* Avatar upload dialog */}
      <Dialog open={avatarDialogOpen} onClose={() => setAvatarDialogOpen(false)}>
        <DialogTitle>Upload Profile Picture</DialogTitle>
        <DialogContent>
          <Typography variant="body2" sx={{ mb: 2 }}>
            Select a new profile picture to upload. Image should be square for best results.
          </Typography>
          
          <Button
            variant="contained"
            component="label"
            startIcon={<PhotoCameraIcon />}
          >
            Select Image
            <input
              type="file"
              accept="image/*"
              hidden
              onChange={handleAvatarUpload}
            />
          </Button>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setAvatarDialogOpen(false)}>
            Cancel
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default Profile;
