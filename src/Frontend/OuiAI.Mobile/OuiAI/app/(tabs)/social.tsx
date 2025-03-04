import React, { useState } from 'react';
import { StyleSheet, View, FlatList } from 'react-native';
import { Text, Appbar, Divider, Avatar, Button, Searchbar, ActivityIndicator } from 'react-native-paper';
import { createMaterialTopTabNavigator } from '@react-navigation/material-top-tabs';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { router } from 'expo-router';

import { Colors } from '@/constants/Colors';
import { useColorScheme } from '@/hooks/useColorScheme';

const Tab = createMaterialTopTabNavigator();

// Mock data for demonstration
const mockFollowers = Array.from({ length: 20 }, (_, i) => ({
  id: `follower-${i}`,
  username: `user${i}`,
  displayName: `User ${i}`,
  profileImageUrl: `https://i.pravatar.cc/150?u=${i}`,
  followedAt: new Date(Date.now() - Math.random() * 10000000000).toISOString(),
}));

const mockFollowing = Array.from({ length: 15 }, (_, i) => ({
  id: `following-${i}`,
  username: `following${i}`,
  displayName: `Following ${i}`,
  profileImageUrl: `https://i.pravatar.cc/150?u=${i + 100}`,
  followedAt: new Date(Date.now() - Math.random() * 10000000000).toISOString(),
}));

const mockActivities = Array.from({ length: 25 }, (_, i) => ({
  id: `activity-${i}`,
  userId: `user-${i}`,
  username: `user${i}`,
  displayName: `User ${i}`,
  profileImageUrl: `https://i.pravatar.cc/150?u=${i + 200}`,
  activityType: ['post', 'like', 'follow', 'comment'][Math.floor(Math.random() * 4)],
  targetId: `target-${i}`,
  createdAt: new Date(Date.now() - Math.random() * 10000000000).toISOString(),
  description: `Sample activity ${i} description here`,
}));

function FollowItem({ item, isFollowingView = false }) {
  const [isFollowing, setIsFollowing] = useState(isFollowingView);

  const handleFollowToggle = () => {
    setIsFollowing(!isFollowing);
    // TODO: API call to follow/unfollow
  };

  return (
    <View style={styles.followItem}>
      <Avatar.Image source={{ uri: item.profileImageUrl }} size={50} />
      <View style={styles.followItemContent}>
        <Text style={styles.displayName}>{item.displayName}</Text>
        <Text style={styles.username}>@{item.username}</Text>
      </View>
      <Button 
        mode={isFollowing ? "outlined" : "contained"} 
        onPress={handleFollowToggle}
        style={styles.followButton}
        labelStyle={{ fontSize: 12 }}
      >
        {isFollowing ? "Following" : "Follow"}
      </Button>
    </View>
  );
}

function ActivityItem({ item }) {
  const getActivityDescription = () => {
    switch (item.activityType) {
      case 'post':
        return 'shared a new post';
      case 'like':
        return 'liked a post';
      case 'follow':
        return 'started following someone';
      case 'comment':
        return 'commented on a post';
      default:
        return 'did something';
    }
  };

  return (
    <View style={styles.activityItem}>
      <Avatar.Image source={{ uri: item.profileImageUrl }} size={50} />
      <View style={styles.activityItemContent}>
        <Text style={styles.displayName}>{item.displayName}</Text>
        <Text>{getActivityDescription()}</Text>
        <Text style={styles.timestamp}>
          {new Date(item.createdAt).toLocaleDateString()}
        </Text>
      </View>
    </View>
  );
}

function ActivityFeed() {
  const [refreshing, setRefreshing] = useState(false);
  const [loading, setLoading] = useState(false);
  const [activities, setActivities] = useState(mockActivities);

  const handleRefresh = () => {
    setRefreshing(true);
    // TODO: API call to refresh data
    setTimeout(() => {
      setRefreshing(false);
    }, 1500);
  };

  const handleLoadMore = () => {
    if (loading) return;
    setLoading(true);
    // TODO: API call to load more data
    setTimeout(() => {
      setLoading(false);
    }, 1500);
  };

  return (
    <FlatList
      data={activities}
      keyExtractor={(item) => item.id}
      renderItem={({ item }) => <ActivityItem item={item} />}
      ItemSeparatorComponent={() => <Divider />}
      refreshing={refreshing}
      onRefresh={handleRefresh}
      onEndReached={handleLoadMore}
      onEndReachedThreshold={0.5}
      ListFooterComponent={loading ? <ActivityIndicator style={{ padding: 10 }} /> : null}
    />
  );
}

function FollowersTab() {
  const [refreshing, setRefreshing] = useState(false);
  const [loading, setLoading] = useState(false);
  const [followers, setFollowers] = useState(mockFollowers);
  const [searchQuery, setSearchQuery] = useState('');

  const handleRefresh = () => {
    setRefreshing(true);
    // TODO: API call to refresh data
    setTimeout(() => {
      setRefreshing(false);
    }, 1500);
  };

  const handleLoadMore = () => {
    if (loading) return;
    setLoading(true);
    // TODO: API call to load more data
    setTimeout(() => {
      setLoading(false);
    }, 1500);
  };

  const filteredFollowers = followers.filter(follower => 
    follower.displayName.toLowerCase().includes(searchQuery.toLowerCase()) ||
    follower.username.toLowerCase().includes(searchQuery.toLowerCase())
  );

  return (
    <View style={styles.container}>
      <Searchbar
        placeholder="Search followers"
        onChangeText={setSearchQuery}
        value={searchQuery}
        style={styles.searchBar}
      />
      <FlatList
        data={filteredFollowers}
        keyExtractor={(item) => item.id}
        renderItem={({ item }) => <FollowItem item={item} />}
        ItemSeparatorComponent={() => <Divider />}
        refreshing={refreshing}
        onRefresh={handleRefresh}
        onEndReached={handleLoadMore}
        onEndReachedThreshold={0.5}
        ListFooterComponent={loading ? <ActivityIndicator style={{ padding: 10 }} /> : null}
      />
    </View>
  );
}

function FollowingTab() {
  const [refreshing, setRefreshing] = useState(false);
  const [loading, setLoading] = useState(false);
  const [following, setFollowing] = useState(mockFollowing);
  const [searchQuery, setSearchQuery] = useState('');

  const handleRefresh = () => {
    setRefreshing(true);
    // TODO: API call to refresh data
    setTimeout(() => {
      setRefreshing(false);
    }, 1500);
  };

  const handleLoadMore = () => {
    if (loading) return;
    setLoading(true);
    // TODO: API call to load more data
    setTimeout(() => {
      setLoading(false);
    }, 1500);
  };

  const filteredFollowing = following.filter(follow => 
    follow.displayName.toLowerCase().includes(searchQuery.toLowerCase()) ||
    follow.username.toLowerCase().includes(searchQuery.toLowerCase())
  );

  return (
    <View style={styles.container}>
      <Searchbar
        placeholder="Search following"
        onChangeText={setSearchQuery}
        value={searchQuery}
        style={styles.searchBar}
      />
      <FlatList
        data={filteredFollowing}
        keyExtractor={(item) => item.id}
        renderItem={({ item }) => <FollowItem item={item} isFollowingView={true} />}
        ItemSeparatorComponent={() => <Divider />}
        refreshing={refreshing}
        onRefresh={handleRefresh}
        onEndReached={handleLoadMore}
        onEndReachedThreshold={0.5}
        ListFooterComponent={loading ? <ActivityIndicator style={{ padding: 10 }} /> : null}
      />
    </View>
  );
}

export default function SocialScreen() {
  const colorScheme = useColorScheme();
  const insets = useSafeAreaInsets();

  return (
    <View style={[styles.container, { paddingTop: insets.top }]}>
      <Appbar.Header>
        <Appbar.Content title="Social" />
        <Appbar.Action icon="magnify" onPress={() => {}} />
        <Appbar.Action icon="bell-outline" onPress={() => router.push('/notifications')} />
      </Appbar.Header>
      
      <Tab.Navigator
        screenOptions={{
          tabBarActiveTintColor: Colors[colorScheme ?? 'light'].tint,
          tabBarIndicatorStyle: {
            backgroundColor: Colors[colorScheme ?? 'light'].tint,
          },
        }}>
        <Tab.Screen name="Activity" component={ActivityFeed} />
        <Tab.Screen name="Followers" component={FollowersTab} />
        <Tab.Screen name="Following" component={FollowingTab} />
      </Tab.Navigator>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#fff',
  },
  searchBar: {
    margin: 8,
    borderRadius: 10,
  },
  followItem: {
    flexDirection: 'row',
    padding: 16,
    alignItems: 'center',
  },
  followItemContent: {
    flex: 1,
    marginLeft: 15,
  },
  displayName: {
    fontWeight: 'bold',
    fontSize: 16,
  },
  username: {
    color: '#777',
  },
  timestamp: {
    color: '#777',
    fontSize: 12,
    marginTop: 4,
  },
  followButton: {
    borderRadius: 20,
  },
  activityItem: {
    flexDirection: 'row',
    padding: 16,
  },
  activityItemContent: {
    flex: 1,
    marginLeft: 15,
  },
});
