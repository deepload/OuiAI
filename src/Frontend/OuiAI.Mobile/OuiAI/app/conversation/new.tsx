import React, { useState, useEffect } from 'react';
import { StyleSheet, View, FlatList, TouchableOpacity } from 'react-native';
import { Text, Appbar, Searchbar, Checkbox, Avatar, Button, Divider, Chip } from 'react-native-paper';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { router } from 'expo-router';

import { Colors } from '@/constants/Colors';
import { useColorScheme } from '@/hooks/useColorScheme';

// Mock data for demonstration
const mockUsers = Array.from({ length: 20 }, (_, i) => ({
  id: `user-${i}`,
  username: `user${i}`,
  displayName: `User ${i}`,
  profileImageUrl: `https://i.pravatar.cc/150?u=${i}`,
  isFollowing: i < 10, // First 10 users are being followed by current user
}));

function UserItem({ user, selected, onToggle }) {
  return (
    <TouchableOpacity 
      style={styles.userItem} 
      onPress={() => onToggle(user)}
    >
      <View style={styles.userInfo}>
        <Avatar.Image source={{ uri: user.profileImageUrl }} size={48} />
        <View style={styles.userDetails}>
          <Text style={styles.displayName}>{user.displayName}</Text>
          <Text style={styles.username}>@{user.username}</Text>
        </View>
      </View>
      <Checkbox
        status={selected ? 'checked' : 'unchecked'}
        onPress={() => onToggle(user)}
      />
    </TouchableOpacity>
  );
}

export default function NewConversationScreen() {
  const colorScheme = useColorScheme();
  const insets = useSafeAreaInsets();
  const [searchQuery, setSearchQuery] = useState('');
  const [users, setUsers] = useState(mockUsers);
  const [selectedUsers, setSelectedUsers] = useState([]);
  const [followingOnly, setFollowingOnly] = useState(true);

  // Filter users based on search and following status
  const filteredUsers = users.filter(user => {
    const matchesSearch = 
      user.displayName.toLowerCase().includes(searchQuery.toLowerCase()) ||
      user.username.toLowerCase().includes(searchQuery.toLowerCase());
    
    if (followingOnly) {
      return matchesSearch && user.isFollowing;
    }
    
    return matchesSearch;
  });

  const toggleUserSelection = (user) => {
    if (selectedUsers.some(u => u.id === user.id)) {
      setSelectedUsers(selectedUsers.filter(u => u.id !== user.id));
    } else {
      setSelectedUsers([...selectedUsers, user]);
    }
  };

  const createConversation = () => {
    if (selectedUsers.length === 0) return;
    
    // In a real app, you would call the API to create a conversation
    // For now, we'll simulate and redirect to a new conversation
    const newConversationId = `new-${Date.now()}`;
    
    router.push({
      pathname: '/conversation/[id]',
      params: { id: newConversationId }
    });
  };

  const removeSelectedUser = (user) => {
    setSelectedUsers(selectedUsers.filter(u => u.id !== user.id));
  };

  return (
    <View style={[styles.container, { paddingTop: insets.top }]}>
      <Appbar.Header>
        <Appbar.BackAction onPress={() => router.back()} />
        <Appbar.Content title="New Conversation" />
        {selectedUsers.length > 0 && (
          <Appbar.Action 
            icon="check" 
            onPress={createConversation} 
          />
        )}
      </Appbar.Header>

      {selectedUsers.length > 0 && (
        <View style={styles.selectedUsersContainer}>
          <FlatList
            data={selectedUsers}
            keyExtractor={(item) => item.id}
            horizontal
            renderItem={({ item }) => (
              <Chip
                avatar={<Avatar.Image size={24} source={{ uri: item.profileImageUrl }} />}
                onClose={() => removeSelectedUser(item)}
                style={styles.userChip}
              >
                {item.displayName}
              </Chip>
            )}
            contentContainerStyle={styles.selectedUsersList}
          />
        </View>
      )}
      
      <Searchbar
        placeholder="Search users"
        onChangeText={setSearchQuery}
        value={searchQuery}
        style={styles.searchBar}
      />
      
      <View style={styles.filterContainer}>
        <TouchableOpacity 
          style={styles.filterOption} 
          onPress={() => setFollowingOnly(true)}
        >
          <Text style={[
            styles.filterText, 
            followingOnly && { color: Colors[colorScheme ?? 'light'].tint }
          ]}>
            Following
          </Text>
          {followingOnly && (
            <View style={[
              styles.activeFilterIndicator,
              { backgroundColor: Colors[colorScheme ?? 'light'].tint }
            ]} />
          )}
        </TouchableOpacity>
        
        <TouchableOpacity 
          style={styles.filterOption} 
          onPress={() => setFollowingOnly(false)}
        >
          <Text style={[
            styles.filterText, 
            !followingOnly && { color: Colors[colorScheme ?? 'light'].tint }
          ]}>
            All Users
          </Text>
          {!followingOnly && (
            <View style={[
              styles.activeFilterIndicator,
              { backgroundColor: Colors[colorScheme ?? 'light'].tint }
            ]} />
          )}
        </TouchableOpacity>
      </View>
      
      <FlatList
        data={filteredUsers}
        keyExtractor={(item) => item.id}
        renderItem={({ item }) => (
          <UserItem 
            user={item} 
            selected={selectedUsers.some(u => u.id === item.id)}
            onToggle={toggleUserSelection}
          />
        )}
        ItemSeparatorComponent={() => <Divider />}
      />
      
      {selectedUsers.length > 0 && (
        <View style={styles.createButtonContainer}>
          <Button 
            mode="contained" 
            onPress={createConversation}
            style={styles.createButton}
          >
            Start Conversation with {selectedUsers.length} {selectedUsers.length === 1 ? 'User' : 'Users'}
          </Button>
        </View>
      )}
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
  selectedUsersContainer: {
    backgroundColor: '#F5F5F5',
    padding: 8,
  },
  selectedUsersList: {
    paddingVertical: 4,
  },
  userChip: {
    marginHorizontal: 4,
  },
  filterContainer: {
    flexDirection: 'row',
    borderBottomWidth: 1,
    borderBottomColor: '#E0E0E0',
  },
  filterOption: {
    flex: 1,
    paddingVertical: 12,
    alignItems: 'center',
    position: 'relative',
  },
  filterText: {
    fontWeight: '600',
  },
  activeFilterIndicator: {
    position: 'absolute',
    bottom: 0,
    height: 3,
    width: '80%',
    borderRadius: 1.5,
  },
  userItem: {
    flexDirection: 'row',
    padding: 16,
    alignItems: 'center',
    justifyContent: 'space-between',
  },
  userInfo: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  userDetails: {
    marginLeft: 16,
  },
  displayName: {
    fontWeight: 'bold',
    fontSize: 16,
  },
  username: {
    color: '#777',
  },
  createButtonContainer: {
    padding: 16,
    borderTopWidth: 1,
    borderTopColor: '#E0E0E0',
  },
  createButton: {
    borderRadius: 10,
  },
});
