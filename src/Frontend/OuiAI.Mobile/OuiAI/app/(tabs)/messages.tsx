import React, { useState } from 'react';
import { StyleSheet, View, FlatList, TouchableOpacity } from 'react-native';
import { Text, Appbar, Divider, Avatar, Badge, Searchbar, FAB, ActivityIndicator } from 'react-native-paper';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { router } from 'expo-router';
import dayjs from 'dayjs';
import relativeTime from 'dayjs/plugin/relativeTime';

import { Colors } from '@/constants/Colors';
import { useColorScheme } from '@/hooks/useColorScheme';

// Add relative time plugin to dayjs
dayjs.extend(relativeTime);

// Define types
interface User {
  id: string;
  username: string;
  displayName: string;
  profileImageUrl: string;
}

interface Message {
  id: string;
  senderId: string;
  content: string;
  createdAt: string;
  isRead: boolean;
}

interface Conversation {
  id: string;
  title: string | null;
  participants: User[];
  lastMessage: Message;
  unreadCount: number;
}

interface ConversationItemProps {
  conversation: Conversation;
  onPress: (conversation: Conversation) => void;
}

// Mock data for demonstration
const mockConversations: Conversation[] = Array.from({ length: 15 }, (_, i) => ({
  id: `conversation-${i}`,
  title: i % 3 === 0 ? `Group Chat ${Math.floor(i/3)}` : null,
  participants: i % 3 === 0 
    ? Array.from({ length: Math.floor(Math.random() * 5) + 2 }, (_, j) => ({
        id: `participant-${i}-${j}`,
        username: `user${j}`,
        displayName: `User ${j}`,
        profileImageUrl: `https://i.pravatar.cc/150?u=${i*10 + j}`,
      })) 
    : [{
        id: `participant-${i}-0`,
        username: `user${i}`,
        displayName: `User ${i}`,
        profileImageUrl: `https://i.pravatar.cc/150?u=${i}`,
      }],
  lastMessage: {
    id: `message-${i}`,
    senderId: i % 2 === 0 ? 'current-user' : `participant-${i}-0`,
    content: `This is message number ${i} in the conversation. Just some random text to show how it would look.`,
    createdAt: new Date(Date.now() - Math.random() * 10000000000).toISOString(),
    isRead: i > 5,
  },
  unreadCount: i > 5 ? 0 : Math.floor(Math.random() * 10) + 1,
}));

function ConversationItem({ conversation, onPress }: ConversationItemProps) {
  const hasUnread = conversation.unreadCount > 0;
  const isGroupChat = conversation.participants.length > 1;
  const participant = isGroupChat ? null : conversation.participants[0];
  
  const displayName = isGroupChat 
    ? conversation.title
    : participant?.displayName;
  
  const avatarSource = isGroupChat 
    ? require('@/assets/images/group-avatar.png') 
    : { uri: participant?.profileImageUrl };

  const isFromMe = conversation.lastMessage.senderId === 'current-user';
  const timeAgo = dayjs(conversation.lastMessage.createdAt).fromNow();

  return (
    <TouchableOpacity 
      style={[styles.conversationItem, hasUnread && styles.unreadConversation]} 
      onPress={() => onPress(conversation)}>
      <View style={styles.avatarContainer}>
        <Avatar.Image source={avatarSource} size={55} />
        {hasUnread && (
          <Badge style={styles.unreadBadge}>{conversation.unreadCount}</Badge>
        )}
      </View>
      
      <View style={styles.conversationContent}>
        <View style={styles.conversationHeader}>
          <Text style={[styles.displayName, hasUnread && styles.unreadText]}>{displayName}</Text>
          <Text style={styles.timeAgo}>{timeAgo}</Text>
        </View>
        
        <View style={styles.lastMessageContainer}>
          {isFromMe && <Text style={styles.fromMe}>You: </Text>}
          <Text 
            style={[styles.lastMessage, hasUnread && styles.unreadText]} 
            numberOfLines={2}
            ellipsizeMode="tail">
            {conversation.lastMessage.content}
          </Text>
        </View>
      </View>
    </TouchableOpacity>
  );
}

export default function MessagesScreen() {
  const colorScheme = useColorScheme();
  const insets = useSafeAreaInsets();
  const [refreshing, setRefreshing] = useState(false);
  const [loading, setLoading] = useState(false);
  const [conversations, setConversations] = useState<Conversation[]>(mockConversations);
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

  const handleConversationPress = (conversation: Conversation) => {
    router.push({
      pathname: '/conversation/[id]',
      params: { id: conversation.id }
    });
  };

  const handleNewMessage = () => {
    router.push('/conversation/new');
  };

  const filteredConversations = conversations.filter(conversation => {
    const title = conversation.title || '';
    const participantNames = conversation.participants.map(p => p.displayName).join(' ');
    const searchTerm = searchQuery.toLowerCase();
    
    return title.toLowerCase().includes(searchTerm) || 
           participantNames.toLowerCase().includes(searchTerm) ||
           conversation.lastMessage.content.toLowerCase().includes(searchTerm);
  });

  return (
    <View style={[styles.container, { paddingTop: insets.top }]}>
      <Appbar.Header>
        <Appbar.Content title="Messages" />
        <Appbar.Action icon="magnify" onPress={() => {}} />
      </Appbar.Header>
      
      <Searchbar
        placeholder="Search messages"
        onChangeText={setSearchQuery}
        value={searchQuery}
        style={styles.searchBar}
      />
      
      <FlatList
        data={filteredConversations}
        keyExtractor={(item) => item.id}
        renderItem={({ item }) => (
          <ConversationItem 
            conversation={item} 
            onPress={handleConversationPress}
          />
        )}
        ItemSeparatorComponent={() => <Divider />}
        refreshing={refreshing}
        onRefresh={handleRefresh}
        onEndReached={handleLoadMore}
        onEndReachedThreshold={0.5}
        ListFooterComponent={loading ? <ActivityIndicator style={{ padding: 10 }} /> : null}
      />
      
      <FAB
        icon="message-plus"
        style={[styles.fab, { backgroundColor: Colors[colorScheme ?? 'light'].tint }]}
        onPress={handleNewMessage}
      />
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
  conversationItem: {
    flexDirection: 'row',
    padding: 12,
    alignItems: 'center',
  },
  unreadConversation: {
    backgroundColor: '#F0F7FF',
  },
  avatarContainer: {
    position: 'relative',
    marginRight: 15,
  },
  unreadBadge: {
    position: 'absolute',
    top: 0,
    right: -5,
  },
  conversationContent: {
    flex: 1,
  },
  conversationHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'baseline',
    marginBottom: 4,
  },
  displayName: {
    fontWeight: 'bold',
    fontSize: 16,
  },
  unreadText: {
    fontWeight: 'bold',
  },
  timeAgo: {
    fontSize: 12,
    color: '#777',
  },
  lastMessageContainer: {
    flexDirection: 'row',
  },
  fromMe: {
    fontStyle: 'italic',
  },
  lastMessage: {
    flex: 1,
    color: '#555',
  },
  fab: {
    position: 'absolute',
    margin: 16,
    right: 0,
    bottom: 0,
  },
});
