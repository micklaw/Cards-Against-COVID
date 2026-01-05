import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
import { gameApi } from '../api/gameApi';
import type { ChatMessage } from '../types/game';
import { setChatVisitCookie } from '../utils/cookies';

interface ChatState {
  messages: ChatMessage[];
  fetching: boolean;
  error: string | null;
  hasMore: boolean;
}

const initialState: ChatState = {
  messages: [],
  fetching: false,
  error: null,
  hasMore: true,
};

// Async thunks
export const fetchInitialMessages = createAsyncThunk(
  'chat/fetchInitial',
  async (gameUrl: string) => {
    return await gameApi.getChatMessages(gameUrl, { limit: 20 });
  }
);

export const fetchOlderMessages = createAsyncThunk(
  'chat/fetchOlder',
  async ({ gameUrl, beforeMessageId }: { gameUrl: string; beforeMessageId: string }) => {
    return await gameApi.getChatMessages(gameUrl, { beforeMessageId, limit: 20 });
  }
);

export const fetchNewerMessages = createAsyncThunk(
  'chat/fetchNewer',
  async ({ gameUrl, afterMessageId }: { gameUrl: string; afterMessageId: string }) => {
    return await gameApi.getChatMessages(gameUrl, { afterMessageId, limit: 20 });
  }
);

export const postMessage = createAsyncThunk(
  'chat/postMessage',
  async ({ gameUrl, userId, content, quotedMessageId }: { 
    gameUrl: string; 
    userId: number; 
    content: string; 
    quotedMessageId?: string;
  }) => {
    return await gameApi.postChatMessage(gameUrl, userId, content, quotedMessageId);
  }
);

const chatSlice = createSlice({
  name: 'chat',
  initialState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    },
    resetChat: (state) => {
      state.messages = [];
      state.error = null;
      state.hasMore = true;
    },
    markAsVisited: (_state, action: PayloadAction<string>) => {
      // Set cookie when marking as visited
      setChatVisitCookie(action.payload);
    },
  },
  extraReducers: (builder) => {
    // Fetch initial messages
    builder.addCase(fetchInitialMessages.pending, (state) => {
      state.fetching = true;
      state.error = null;
    });
    builder.addCase(fetchInitialMessages.fulfilled, (state, action) => {
      state.fetching = false;
      state.messages = action.payload;
      state.hasMore = action.payload.length === 20;
    });
    builder.addCase(fetchInitialMessages.rejected, (state, action) => {
      state.fetching = false;
      state.error = action.error.message || 'Failed to fetch messages';
    });

    // Fetch older messages
    builder.addCase(fetchOlderMessages.pending, (state) => {
      state.fetching = true;
      state.error = null;
    });
    builder.addCase(fetchOlderMessages.fulfilled, (state, action) => {
      state.fetching = false;
      // Prepend older messages
      state.messages = [...action.payload, ...state.messages];
      state.hasMore = action.payload.length === 20;
    });
    builder.addCase(fetchOlderMessages.rejected, (state, action) => {
      state.fetching = false;
      state.error = action.error.message || 'Failed to fetch older messages';
    });

    // Fetch newer messages
    builder.addCase(fetchNewerMessages.fulfilled, (state, action) => {
      // Append newer messages and deduplicate
      const newMessages = action.payload.filter(
        newMsg => !state.messages.some(msg => msg.messageId === newMsg.messageId)
      );
      state.messages = [...state.messages, ...newMessages];
    });

    // Post message
    builder.addCase(postMessage.fulfilled, (state, action) => {
      // Add the new message if not already present
      if (!state.messages.some(msg => msg.messageId === action.payload.messageId)) {
        state.messages.push(action.payload);
      }
    });
    builder.addCase(postMessage.rejected, (state, action) => {
      state.error = action.error.message || 'Failed to send message';
    });
  },
});

export const { clearError, resetChat, markAsVisited } = chatSlice.actions;

// Selectors
export const selectMessages = (state: { chat: ChatState }) => state.chat.messages;
export const selectChatFetching = (state: { chat: ChatState }) => state.chat.fetching;
export const selectChatError = (state: { chat: ChatState }) => state.chat.error;
export const selectHasMore = (state: { chat: ChatState }) => state.chat.hasMore;

export default chatSlice.reducer;
