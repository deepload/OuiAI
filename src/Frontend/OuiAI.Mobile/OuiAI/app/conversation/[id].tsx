import React, { useState, useEffect, useRef } from 'react';
import { StyleSheet, View, TextInput, FlatList, KeyboardAvoidingView, Platform, TouchableOpacity, Keyboard } from 'react-native';
import { Text, Appbar, Avatar, ActivityIndicator, IconButton, useTheme } from 'react-native-paper';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { useLocalSearchParams, router } from 'expo-router';
import dayjs from 'dayjs';
import * as Haptics from 'expo-haptics';

import { Colors } from '@/constants/Colors';
import { useColorScheme } from '@/hooks/useColorScheme';

// Mock data for conversation
const mockMessages = Array.from({ length: 25 }, (_, i) => ({
  id: `message-${i}`,
  conversationId: 'conversation-1',
  content: `This is message number ${i} in the conversation. ${
    i % 5 === 0 ? 'A longer message to show how text wrapping works in the chat bubbles. This should demonstrate multi-line capability.' : ''
  }`,
  senderId: i % 3 === 0 ? 'current-user' : `other-user-${i % 2}`,
  senderName: i % 3 === 0 ? 'You' : `User ${i % 2}`,
  senderAvatar: i % 3 === 0 ? null : `https://i.pravatar.cc/150?u=${i % 5}`,
  createdAt: new Date(Date.now() - (25 - i) * 3600000).toISOString(),
  isRead: true,
})).reverse(); // Reverse to show newest at the bottom

const mockConversation = {
  id: 'conversation-1',
  title: 'Project Discussion',
  isGroupChat: true,
  participants: [
    { id: 'user-1', username: 'user1', displayName: 'John Doe', profileImageUrl: 'https://i.pravatar.cc/150?u=1' },
    { id: 'user-2', username: 'user2', displayName: 'Jane Smith', profileImageUrl: 'https://i.pravatar.cc/150?u=2' },
    { id: 'user-3', username: 'user3', displayName: 'Bob Johnson', profileImageUrl: 'https://i.pravatar.cc/150?u=3' },
  ],
};

const MessageItem = ({ message, previousMessage, showAvatar = true }) => {
  const isFromMe = message.senderId === 'current-user';
  const sameAsPrevious = previousMessage && previousMessage.senderId === message.senderId;
  const messageTime = dayjs(message.createdAt).format('HH:mm');
  
  // Determine if we should show the avatar (for non-user messages)
  const shouldShowAvatar = !isFromMe && showAvatar && !sameAsPrevious;

  return (
    <View style={[
      styles.messageRow,
      isFromMe ? styles.myMessageRow : styles.theirMessageRow
    ]}>
      {!isFromMe && (
        <View style={styles.avatarContainer}>
          {shouldShowAvatar ? (
            <Avatar.Image 
              size={36} 
              source={{ uri: message.senderAvatar }} 
              style={styles.avatar} 
            />
          ) : (
            <View style={{ width: 36 }} />
          )}
        </View>
      )}

      <View>
        {!isFromMe && !sameAsPrevious && (
          <Text style={styles.messageSender}>{message.senderName}</Text>
        )}
        
        <View style={[
          styles.messageBubble,
          isFromMe ? styles.myMessage : styles.theirMessage,
          sameAsPrevious ? (isFromMe ? styles.myGroupedMessage : styles.theirGroupedMessage) : null
        ]}>
          <Text>{message.content}</Text>
          <Text style={styles.messageTime}>{messageTime}</Text>
        </View>
      </View>
    </View>
  );
};

export default function ConversationDetail() {
  const { id } = useLocalSearchParams();
  const colorScheme = useColorScheme();
  const insets = useSafeAreaInsets();
  const theme = useTheme();
  const [conversation, setConversation] = useState(mockConversation);
  const [messages, setMessages] = useState(mockMessages);
  const [loading, setLoading] = useState(false);
  const [inputText, setInputText] = useState('');
  const [isTyping, setIsTyping] = useState({});
  const flatListRef = useRef(null);
  const inputRef = useRef(null);

  // Fetch conversation data
  useEffect(() => {
    // In a real app, you would fetch conversation and messages from the API
    // For now, we'll use mock data
    console.log(`Fetching conversation ${id}`);
  }, [id]);

  const handleSendMessage = () => {
    if (!inputText.trim()) return;
    
    const newMessage = {
      id: `new-message-${Date.now()}`,
      conversationId: conversation.id,
      content: inputText.trim(),
      senderId: 'current-user',
      senderName: 'You',
      createdAt: new Date().toISOString(),
      isRead: false,
    };

    // Add message to UI immediately for better UX
    setMessages([newMessage, ...messages]);
    
    // Clear input
    setInputText('');
    
    // Provide haptic feedback
    Haptics.impactAsync(Haptics.ImpactFeedbackStyle.Light);
    
    // In a real app, you would send the message to the API and handle SignalR updates
    // For now, we'll just update the UI
    
    // Scroll to the bottom to show the new message
    setTimeout(() => {
      flatListRef.current?.scrollToOffset({ offset: 0, animated: true });
    }, 100);
  };

  const handleLoadMoreMessages = () => {
    if (loading) return;
    setLoading(true);
    
    // In a real app, you would fetch more messages from the API
    // For demonstration, we'll simulate a delay and add more messages
    setTimeout(() => {
      const moreMessages = Array.from({ length: 10 }, (_, i) => ({
        id: `older-message-${Date.now()}-${i}`,
        conversationId: conversation.id,
        content: `This is an older message ${i}`,
        senderId: i % 3 === 0 ? 'current-user' : `other-user-${i % 2}`,
        senderName: i % 3 === 0 ? 'You' : `User ${i % 2}`,
        senderAvatar: i % 3 === 0 ? null : `https://i.pravatar.cc/150?u=${i % 5}`,
        createdAt: new Date(Date.now() - messages.length * 3600000 - i * 3600000).toISOString(),
        isRead: true,
      }));
      
      setMessages([...messages, ...moreMessages]);
      setLoading(false);
    }, 1000);
  };

  const getConversationTitle = () => {
    if (conversation.title) return conversation.title;
    if (conversation.participants.length === 1) {
      return conversation.participants[0].displayName;
    }
    return conversation.participants
      .map((p) => p.displayName.split(' ')[0])
      .join(', ');
  };

  return (
    <KeyboardAvoidingView
      style={[styles.container, { paddingTop: insets.top }]}
      behavior={Platform.OS === 'ios' ? 'padding' : undefined}
      keyboardVerticalOffset={Platform.OS === 'ios' ? 64 : 0}
    >
      <Appbar.Header>
        <Appbar.BackAction onPress={() => router.back()} />
        <Appbar.Content 
          title={getConversationTitle()} 
          subtitle={conversation.isGroupChat 
            ? `${conversation.participants.length} participants` 
            : ''
          } 
        />
        <Appbar.Action icon="information-outline" onPress={() => {}} />
      </Appbar.Header>

      <FlatList
        ref={flatListRef}
        data={messages}
        keyExtractor={(item) => item.id}
        renderItem={({ item, index }) => (
          <MessageItem 
            message={item} 
            previousMessage={index < messages.length - 1 ? messages[index + 1] : null}
          />
        )}
        inverted
        onEndReached={handleLoadMoreMessages}
        onEndReachedThreshold={0.2}
        ListFooterComponent={loading ? (
          <ActivityIndicator style={{ padding: 20 }} />
        ) : null}
        contentContainerStyle={{ paddingBottom: 10 }}
      />

      {Object.keys(isTyping).length > 0 && (
        <View style={styles.typingIndicator}>
          <Text style={styles.typingText}>
            {Object.values(isTyping).join(', ')} typing...
          </Text>
        </View>
      )}
      
      <View style={styles.inputContainer}>
        <IconButton
          icon="paperclip"
          size={24}
          onPress={() => {}}
          style={styles.attachButton}
        />
        <TextInput
          ref={inputRef}
          style={styles.input}
          placeholder="Type a message..."
          value={inputText}
          onChangeText={setInputText}
          multiline
        />
        <IconButton
          icon="send"
          size={24}
          iconColor={Colors[colorScheme ?? 'light'].tint}
          disabled={!inputText.trim()}
          onPress={handleSendMessage}
          style={styles.sendButton}
        />
      </View>
    </KeyboardAvoidingView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
  },
  messageRow: {
    flexDirection: 'row',
    marginVertical: 2,
    paddingHorizontal: 16,
  },
  myMessageRow: {
    justifyContent: 'flex-end',
  },
  theirMessageRow: {
    justifyContent: 'flex-start',
  },
  avatarContainer: {
    width: 36,
    marginRight: 8,
    alignItems: 'center',
    justifyContent: 'flex-end',
  },
  avatar: {
    marginBottom: 4,
  },
  messageSender: {
    fontSize: 12,
    color: '#666',
    marginLeft: 12,
    marginBottom: 2,
  },
  messageBubble: {
    maxWidth: '80%',
    padding: 12,
    borderRadius: 18,
    marginBottom: 2,
    position: 'relative',
  },
  myMessage: {
    backgroundColor: '#DCF8C6',
    borderTopRightRadius: 4,
    marginLeft: 40,
  },
  theirMessage: {
    backgroundColor: '#FFFFFF',
    borderTopLeftRadius: 4,
  },
  myGroupedMessage: {
    borderTopRightRadius: 18,
  },
  theirGroupedMessage: {
    borderTopLeftRadius: 18,
  },
  messageTime: {
    fontSize: 10,
    color: '#999',
    alignSelf: 'flex-end',
    marginTop: 4,
  },
  typingIndicator: {
    padding: 8,
    backgroundColor: 'rgba(255,255,255,0.8)',
  },
  typingText: {
    color: '#666',
    fontStyle: 'italic',
  },
  inputContainer: {
    flexDirection: 'row',
    padding: 8,
    backgroundColor: '#FFFFFF',
    alignItems: 'center',
    borderTopWidth: 1,
    borderTopColor: '#E0E0E0',
  },
  attachButton: {
    marginRight: 4,
  },
  input: {
    flex: 1,
    backgroundColor: '#F0F0F0',
    borderRadius: 20,
    paddingHorizontal: 16,
    paddingVertical: 8,
    maxHeight: 100,
  },
  sendButton: {
    marginLeft: 4,
  },
});
