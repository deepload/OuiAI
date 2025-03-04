import React, { useState, useEffect } from 'react';
import { StyleSheet, View, ScrollView, Image, TouchableOpacity, FlatList } from 'react-native';
import { Text, Appbar, Button, ActivityIndicator, Divider, Chip, Avatar } from 'react-native-paper';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { useLocalSearchParams, router } from 'expo-router';
import { createMaterialTopTabNavigator } from '@react-navigation/material-top-tabs';

import { Colors } from '@/constants/Colors';
import { useColorScheme } from '@/hooks/useColorScheme';
import { useAuth } from '@/contexts/AuthContext';
import { socialApi } from '@/services/api/socialApi';

const Tab = createMaterialTopTabNavigator();

// Mock data for user profile
const mockUserProfile = {
  id: 'user-1',
  username: 'jsmith',
  displayName: 'John Smith',
  bio: 'Software engineer passionate about AI and mobile development. Building the future of social networking.',
  profileImageUrl: 'https://i.pravatar.cc/300?u=1',
  coverImageUrl: 'https://picsum.photos/800/400',
  followersCount: 1236,
  followingCount: 427,
  projectsCount: 38,
  skills: ['React Native', 'TypeScript', 'Node.js', 'Azure', '.NET Core', 'AI/ML'],
  isFollowing: false,
  isCurrentUser: false,
};

// Mock activities for the profile
const mockActivities = Array.from({ length: 15 }, (_, i) => ({
  id: `activity-${i}`,
  type: ['project_created', 'project_updated', 'post_created', 'comment_added'][Math.floor(Math.random() * 4)],
  title: `Activity ${i}`,
  description: `This is a description for activity ${i}. It could be longer or shorter depending on the activity.`,
  timestamp: new Date(Date.now() - Math.random() * 10000000000).toISOString(),
}));

function ActivityItem({ activity }) {
  const getActivityIcon = () => {
    switch (activity.type) {
      case 'project_created':
        return 'folder-plus';
      case 'project_updated':
        return 'folder-edit';
      case 'post_created':
        return 'post';
      case 'comment_added':
        return 'comment-text';
      default:
        return 'information';
    }
  };

  const getActivityTitle = () => {
    switch (activity.type) {
      case 'project_created':
        return 'Created a new project';
      case 'project_updated':
        return 'Updated a project';
      case 'post_created':
        return 'Published a post';
      case 'comment_added':
        return 'Commented on a post';
      default:
        return 'Activity';
    }
  };

  return (
    <View style={styles.activityItem}>
      <Avatar.Icon size={40} icon={getActivityIcon()} />
      <View style={styles.activityContent}>
        <Text style={styles.activityTitle}>{getActivityTitle()}</Text>
        <Text>{activity.description}</Text>
        <Text style={styles.timestamp}>
          {new Date(activity.timestamp).toLocaleDateString()}
        </Text>
      </View>
    </View>
  );
}

function ProfileInfo({ profile, onFollowToggle }) {
  const [isFollowing, setIsFollowing] = useState(profile.isFollowing);
  
  const handleFollowToggle = () => {
    setIsFollowing(!isFollowing);
    onFollowToggle(!isFollowing);
  };
  
  return (
    <View style={styles.profileContainer}>
      {/* Cover Image */}
      <Image
        source={{ uri: profile.coverImageUrl }}
        style={styles.coverImage}
      />
      
      {/* Profile Image and Actions */}
      <View style={styles.profileHeader}>
        <Image
          source={{ uri: profile.profileImageUrl }}
          style={styles.profileImage}
        />
        
        {!profile.isCurrentUser ? (
          <View style={styles.actionsContainer}>
            <Button
              mode={isFollowing ? "outlined" : "contained"}
              onPress={handleFollowToggle}
              style={styles.followButton}
            >
              {isFollowing ? "Following" : "Follow"}
            </Button>
            <Button
              mode="outlined"
              icon="message-outline"
              onPress={() => router.push('/conversation/new')}
              style={styles.messageButton}
            >
              Message
            </Button>
          </View>
        ) : (
          <Button
            mode="outlined"
            icon="account-edit"
            onPress={() => router.push('/profile/edit')}
            style={styles.editButton}
          >
            Edit Profile
          </Button>
        )}
      </View>
      
      {/* User Info */}
      <View style={styles.userInfo}>
        <Text style={styles.displayName}>{profile.displayName}</Text>
        <Text style={styles.username}>@{profile.username}</Text>
        <Text style={styles.bio}>{profile.bio}</Text>
      </View>
      
      {/* Stats */}
      <View style={styles.statsContainer}>
        <TouchableOpacity style={styles.stat}>
          <Text style={styles.statValue}>{profile.followersCount}</Text>
          <Text style={styles.statLabel}>Followers</Text>
        </TouchableOpacity>
        
        <TouchableOpacity style={styles.stat}>
          <Text style={styles.statValue}>{profile.followingCount}</Text>
          <Text style={styles.statLabel}>Following</Text>
        </TouchableOpacity>
        
        <TouchableOpacity style={styles.stat}>
          <Text style={styles.statValue}>{profile.projectsCount}</Text>
          <Text style={styles.statLabel}>Projects</Text>
        </TouchableOpacity>
      </View>
      
      {/* Skills */}
      <View style={styles.skillsContainer}>
        <Text style={styles.sectionTitle}>Skills</Text>
        <View style={styles.skillsWrapper}>
          {profile.skills.map((skill, index) => (
            <Chip key={index} style={styles.skillChip}>{skill}</Chip>
          ))}
        </View>
      </View>
    </View>
  );
}

function ActivitiesTab({ userId }) {
  const [loading, setLoading] = useState(false);
  const [activities, setActivities] = useState(mockActivities);
  
  const loadActivities = async () => {
    setLoading(true);
    // In a real app, you would fetch activities from the API
    // For now, we'll use mock data
    setTimeout(() => {
      setLoading(false);
    }, 1000);
  };
  
  useEffect(() => {
    loadActivities();
  }, [userId]);
  
  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" />
      </View>
    );
  }
  
  return (
    <FlatList
      data={activities}
      keyExtractor={(item) => item.id}
      renderItem={({ item }) => <ActivityItem activity={item} />}
      ItemSeparatorComponent={() => <Divider />}
      contentContainerStyle={styles.activitiesList}
    />
  );
}

function ProjectsTab({ userId }) {
  // This would be populated with project information in a real app
  return (
    <View style={styles.loadingContainer}>
      <Text>User's projects will be displayed here</Text>
    </View>
  );
}

export default function ProfileScreen() {
  const { id } = useLocalSearchParams();
  const colorScheme = useColorScheme();
  const insets = useSafeAreaInsets();
  const { user } = useAuth();
  const [loading, setLoading] = useState(true);
  const [profile, setProfile] = useState(null);
  
  // Fetch user profile
  useEffect(() => {
    const fetchUserProfile = async () => {
      setLoading(true);
      try {
        // In a real app, you would fetch the user profile from the API
        // For now, we'll use mock data with a delay to simulate API call
        setTimeout(() => {
          // Check if the profile is the current user's profile
          const isCurrentUser = user && (id === user.id || id === 'me');
          setProfile({
            ...mockUserProfile,
            isCurrentUser,
          });
          setLoading(false);
        }, 1000);
      } catch (error) {
        console.error('Error fetching user profile:', error);
        setLoading(false);
      }
    };
    
    fetchUserProfile();
  }, [id, user]);
  
  const handleFollowToggle = async (shouldFollow) => {
    if (!profile) return;
    
    try {
      if (shouldFollow) {
        await socialApi.follow.followUser(profile.id);
      } else {
        await socialApi.follow.unfollowUser(profile.id);
      }
    } catch (error) {
      console.error('Error toggling follow status:', error);
      // Revert the UI change if the API call fails
      setProfile(prevProfile => ({
        ...prevProfile,
        isFollowing: !shouldFollow,
      }));
    }
  };
  
  if (loading) {
    return (
      <View style={[styles.container, { paddingTop: insets.top }]}>
        <Appbar.Header>
          <Appbar.BackAction onPress={() => router.back()} />
          <Appbar.Content title="Profile" />
        </Appbar.Header>
        <View style={styles.loadingContainer}>
          <ActivityIndicator size="large" />
        </View>
      </View>
    );
  }
  
  if (!profile) {
    return (
      <View style={[styles.container, { paddingTop: insets.top }]}>
        <Appbar.Header>
          <Appbar.BackAction onPress={() => router.back()} />
          <Appbar.Content title="Profile" />
        </Appbar.Header>
        <View style={styles.loadingContainer}>
          <Text>User not found</Text>
        </View>
      </View>
    );
  }
  
  return (
    <View style={[styles.container, { paddingTop: insets.top }]}>
      <Appbar.Header>
        <Appbar.BackAction onPress={() => router.back()} />
        <Appbar.Content title={profile.isCurrentUser ? "My Profile" : "Profile"} />
        {!profile.isCurrentUser && (
          <Appbar.Action icon="dots-vertical" onPress={() => {}} />
        )}
      </Appbar.Header>
      
      <Tab.Navigator
        screenOptions={{
          tabBarActiveTintColor: Colors[colorScheme ?? 'light'].tint,
          tabBarIndicatorStyle: {
            backgroundColor: Colors[colorScheme ?? 'light'].tint,
          },
        }}>
        <Tab.Screen name="Profile">
          {() => (
            <ScrollView>
              <ProfileInfo profile={profile} onFollowToggle={handleFollowToggle} />
            </ScrollView>
          )}
        </Tab.Screen>
        <Tab.Screen name="Activities">
          {() => <ActivitiesTab userId={profile.id} />}
        </Tab.Screen>
        <Tab.Screen name="Projects">
          {() => <ProjectsTab userId={profile.id} />}
        </Tab.Screen>
      </Tab.Navigator>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#fff',
  },
  loadingContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
  profileContainer: {
    padding: 16,
  },
  coverImage: {
    height: 150,
    width: '100%',
    borderRadius: 8,
  },
  profileHeader: {
    flexDirection: 'row',
    alignItems: 'flex-end',
    marginTop: -40,
  },
  profileImage: {
    width: 80,
    height: 80,
    borderRadius: 40,
    borderWidth: 3,
    borderColor: '#fff',
  },
  actionsContainer: {
    flex: 1,
    flexDirection: 'row',
    justifyContent: 'flex-end',
    marginBottom: 8,
  },
  followButton: {
    marginRight: 8,
    borderRadius: 20,
  },
  messageButton: {
    borderRadius: 20,
  },
  editButton: {
    marginLeft: 16,
    marginBottom: 8,
    borderRadius: 20,
  },
  userInfo: {
    marginTop: 16,
  },
  displayName: {
    fontSize: 20,
    fontWeight: 'bold',
  },
  username: {
    color: '#777',
    marginBottom: 8,
  },
  bio: {
    lineHeight: 20,
  },
  statsContainer: {
    flexDirection: 'row',
    marginTop: 16,
    borderWidth: 1,
    borderColor: '#eee',
    borderRadius: 8,
  },
  stat: {
    flex: 1,
    alignItems: 'center',
    paddingVertical: 12,
  },
  statValue: {
    fontSize: 16,
    fontWeight: 'bold',
  },
  statLabel: {
    color: '#777',
    fontSize: 12,
  },
  skillsContainer: {
    marginTop: 16,
  },
  sectionTitle: {
    fontSize: 16,
    fontWeight: 'bold',
    marginBottom: 8,
  },
  skillsWrapper: {
    flexDirection: 'row',
    flexWrap: 'wrap',
  },
  skillChip: {
    margin: 4,
  },
  activitiesList: {
    padding: 16,
  },
  activityItem: {
    flexDirection: 'row',
    padding: 16,
  },
  activityContent: {
    flex: 1,
    marginLeft: 16,
  },
  activityTitle: {
    fontWeight: 'bold',
    marginBottom: 4,
  },
  timestamp: {
    color: '#777',
    fontSize: 12,
    marginTop: 4,
  },
});
