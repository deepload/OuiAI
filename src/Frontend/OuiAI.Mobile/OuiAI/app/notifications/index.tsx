import React, { useState, useEffect } from 'react';
import { StyleSheet, View, FlatList, TouchableOpacity } from 'react-native';
import { Text, Appbar, Divider, Avatar, IconButton, ActivityIndicator, Menu } from 'react-native-paper';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { router } from 'expo-router';
import dayjs from 'dayjs';
import relativeTime from 'dayjs/plugin/relativeTime';

import { Colors } from '@/constants/Colors';
import { useColorScheme } from '@/hooks/useColorScheme';

// Add relative time plugin to dayjs
dayjs.extend(relativeTime);

// Mock data for demonstration
const mockNotifications = Array.from({ length: 20 }, (_, i) => {
  const types = ['follow', 'like', 'comment', 'mention', 'message'];
  const type = types[Math.floor(Math.random() * types.length)];
  
  const baseNotification = {
    id: `notification-${i}`,
    type,
    isRead: i > 5, // First few are unread
    createdAt: new Date(Date.now() - Math.random() * 10000000000).toISOString(),
    actor: {
      id: `user-${i % 5}`,
      username: `user${i % 5}`,
      displayName: `User ${i % 5}`,
      profileImageUrl: `https://i.pravatar.cc/150?u=${i % 5}`,
    },
  };
  
  switch (type) {
    case 'follow':
      return {
        ...baseNotification,
        message: 'started following you',
      };
    case 'like':
      return {
        ...baseNotification,
        message: 'liked your post',
        target: {
          id: `post-${i}`,
          type: 'post',
          preview: 'Post content preview...',
        },
      };
    case 'comment':
      return {
        ...baseNotification,
        message: 'commented on your post',
        target: {
          id: `post-${i}`,
          type: 'post',
          preview: 'Post content preview...',
        },
        comment: 'This is a great post! Thanks for sharing.',
      };
    case 'mention':
      return {
        ...baseNotification,
        message: 'mentioned you in a post',
        target: {
          id: `post-${i}`,
          type: 'post',
          preview: 'Hey @user, check this out!',
        },
      };
    case 'message':
      return {
        ...baseNotification,
        message: 'sent you a message',
        target: {
          id: `conversation-${i}`,
          type: 'conversation',
        },
        preview: 'Hey there! How are you doing today?',
      };
    default:
      return baseNotification;
  }
});

function NotificationItem({ notification, onPress }) {
  const timeAgo = dayjs(notification.createdAt).fromNow();
  
  const getNotificationContent = () => {
    switch (notification.type) {
      case 'follow':
        return (
          <Text>
            <Text style={styles.actorName}>{notification.actor.displayName}</Text>
            <Text> {notification.message}</Text>
          </Text>
        );
      case 'like':
      case 'mention':
        return (
          <View>
            <Text>
              <Text style={styles.actorName}>{notification.actor.displayName}</Text>
              <Text> {notification.message}</Text>
            </Text>
            <Text style={styles.preview} numberOfLines={1}>
              {notification.target.preview}
            </Text>
          </View>
        );
      case 'comment':
        return (
          <View>
            <Text>
              <Text style={styles.actorName}>{notification.actor.displayName}</Text>
              <Text> {notification.message}</Text>
            </Text>
            <Text style={styles.preview} numberOfLines={1}>
              "{notification.comment}"
            </Text>
          </View>
        );
      case 'message':
        return (
          <View>
            <Text>
              <Text style={styles.actorName}>{notification.actor.displayName}</Text>
              <Text> {notification.message}</Text>
            </Text>
            <Text style={styles.preview} numberOfLines={1}>
              {notification.preview}
            </Text>
          </View>
        );
      default:
        return null;
    }
  };

  const handlePress = () => {
    onPress(notification);
    
    // Navigate based on notification type
    switch (notification.type) {
      case 'follow':
        router.push({
          pathname: '/profile/[id]',
          params: { id: notification.actor.id }
        });
        break;
      case 'like':
      case 'comment':
      case 'mention':
        router.push({
          pathname: '/post/[id]',
          params: { id: notification.target.id }
        });
        break;
      case 'message':
        router.push({
          pathname: '/conversation/[id]',
          params: { id: notification.target.id }
        });
        break;
    }
  };

  return (
    <TouchableOpacity 
      style={[
        styles.notificationItem, 
        !notification.isRead && styles.unreadNotification
      ]} 
      onPress={handlePress}
    >
      <Avatar.Image 
        source={{ uri: notification.actor.profileImageUrl }} 
        size={50} 
      />
      
      <View style={styles.notificationContent}>
        {getNotificationContent()}
        <Text style={styles.timeAgo}>{timeAgo}</Text>
      </View>
      
      {!notification.isRead && (
        <View style={styles.unreadIndicator} />
      )}
    </TouchableOpacity>
  );
}

export default function NotificationsScreen() {
  const colorScheme = useColorScheme();
  const insets = useSafeAreaInsets();
  const [notifications, setNotifications] = useState(mockNotifications);
  const [refreshing, setRefreshing] = useState(false);
  const [loading, setLoading] = useState(false);
  const [menuVisible, setMenuVisible] = useState(false);

  const handleRefresh = () => {
    setRefreshing(true);
    // TODO: API call to refresh notifications
    setTimeout(() => {
      setRefreshing(false);
    }, 1500);
  };

  const handleLoadMore = () => {
    if (loading) return;
    setLoading(true);
    // TODO: API call to load more notifications
    setTimeout(() => {
      setLoading(false);
    }, 1500);
  };

  const handleNotificationPress = (notification) => {
    if (!notification.isRead) {
      // Mark as read locally
      setNotifications(notifications.map(n => 
        n.id === notification.id ? { ...n, isRead: true } : n
      ));
      
      // TODO: API call to mark notification as read
    }
  };

  const markAllAsRead = () => {
    setNotifications(notifications.map(n => ({ ...n, isRead: true })));
    setMenuVisible(false);
    // TODO: API call to mark all notifications as read
  };

  const deleteAllNotifications = () => {
    setNotifications([]);
    setMenuVisible(false);
    // TODO: API call to delete all notifications
  };

  return (
    <View style={[styles.container, { paddingTop: insets.top }]}>
      <Appbar.Header>
        <Appbar.BackAction onPress={() => router.back()} />
        <Appbar.Content title="Notifications" />
        <Menu
          visible={menuVisible}
          onDismiss={() => setMenuVisible(false)}
          anchor={
            <Appbar.Action
              icon="dots-vertical"
              onPress={() => setMenuVisible(true)}
            />
          }
        >
          <Menu.Item 
            title="Mark all as read" 
            leadingIcon="check-all"
            onPress={markAllAsRead} 
          />
          <Menu.Item 
            title="Delete all" 
            leadingIcon="delete"
            onPress={deleteAllNotifications} 
          />
        </Menu>
      </Appbar.Header>
      
      <FlatList
        data={notifications}
        keyExtractor={(item) => item.id}
        renderItem={({ item }) => (
          <NotificationItem 
            notification={item}
            onPress={handleNotificationPress}
          />
        )}
        ItemSeparatorComponent={() => <Divider />}
        refreshing={refreshing}
        onRefresh={handleRefresh}
        onEndReached={handleLoadMore}
        onEndReachedThreshold={0.5}
        ListFooterComponent={loading ? (
          <ActivityIndicator style={{ padding: 20 }} />
        ) : null}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#fff',
  },
  notificationItem: {
    flexDirection: 'row',
    padding: 16,
    alignItems: 'center',
  },
  unreadNotification: {
    backgroundColor: '#F0F7FF',
  },
  notificationContent: {
    flex: 1,
    marginLeft: 16,
  },
  actorName: {
    fontWeight: 'bold',
  },
  preview: {
    color: '#666',
    marginTop: 4,
  },
  timeAgo: {
    color: '#777',
    fontSize: 12,
    marginTop: 4,
  },
  unreadIndicator: {
    width: 8,
    height: 8,
    borderRadius: 4,
    backgroundColor: '#3D82F7',
    marginLeft: 8,
  },
});
