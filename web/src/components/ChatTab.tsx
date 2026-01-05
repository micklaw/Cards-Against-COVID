import React, { useEffect, useState, useRef } from 'react';
import { useParams } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import {
  fetchInitialMessages,
  fetchOlderMessages,
  fetchNewerMessages,
  postMessage,
  resetChat,
  markAsVisited,
  selectMessages,
  selectChatFetching,
  selectHasMore,
} from '../store/chatSlice';
import { selectGame, selectCurrentPlayerId } from '../store/gameSlice';

const ChatTab: React.FC = () => {
  const { instance } = useParams<{ instance: string }>();
  const dispatch = useAppDispatch();
  const game = useAppSelector(selectGame);
  const currentPlayerId = useAppSelector(selectCurrentPlayerId);
  const messages = useAppSelector(selectMessages);
  const fetching = useAppSelector(selectChatFetching);
  const hasMore = useAppSelector(selectHasMore);

  const [messageInput, setMessageInput] = useState('');
  const [quotedMessage, setQuotedMessage] = useState<string | null>(null);
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const messagesContainerRef = useRef<HTMLDivElement>(null);

  const isChatDisabled = !game?.isChatEnabled || game?.isOver || !currentPlayerId;

  // Fetch initial messages on mount
  useEffect(() => {
    if (instance) {
      dispatch(resetChat());
      dispatch(fetchInitialMessages(instance));
      dispatch(markAsVisited(instance));
    }
  }, [instance, dispatch]);

  // Auto-scroll to bottom on new messages
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  // Poll for new messages (integrate with existing polling)
  useEffect(() => {
    if (!instance) return;

    const interval = setInterval(() => {
      if (messages.length > 0) {
        const lastMessageId = messages[messages.length - 1].messageId;
        dispatch(fetchNewerMessages({ gameUrl: instance, afterMessageId: lastMessageId }));
      }
    }, 5000); // Poll every 5 seconds

    return () => clearInterval(interval);
  }, [instance, messages, dispatch]);

  const handleLoadOlder = () => {
    if (instance && messages.length > 0 && hasMore && !fetching) {
      const firstMessageId = messages[0].messageId;
      dispatch(fetchOlderMessages({ gameUrl: instance, beforeMessageId: firstMessageId }));
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!instance || !currentPlayerId || !messageInput.trim() || isChatDisabled) {
      return;
    }

    const content = messageInput.trim();
    if (content.length > 140) {
      return;
    }

    try {
      await dispatch(postMessage({
        gameUrl: instance,
        userId: currentPlayerId,
        content,
        quotedMessageId: quotedMessage || undefined,
      })).unwrap();

      setMessageInput('');
      setQuotedMessage(null);
    } catch (error) {
      console.error('Failed to send message:', error);
    }
  };

  const handleReply = (messageId: string) => {
    setQuotedMessage(messageId);
  };

  const stripNestedQuotes = (content: string): string => {
    // Remove existing quote markers (lines starting with >)
    return content.split('\n').filter(line => !line.trim().startsWith('>')).join('\n');
  };

  const truncateQuote = (content: string, maxLength: number = 100): string => {
    const stripped = stripNestedQuotes(content);
    if (stripped.length > maxLength) {
      return stripped.substring(0, maxLength) + '...';
    }
    return stripped;
  };

  const getQuotedMessage = (messageId: string) => {
    return messages.find(m => m.messageId === messageId);
  };

  const getPlayerName = (userId: number): string => {
    const player = game?.players.find(p => p.id === userId);
    return player?.name || 'Unknown';
  };

  const formatTimestamp = (timestamp: string): string => {
    const date = new Date(timestamp);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);

    if (diffMins < 1) return 'Just now';
    if (diffMins < 60) return `${diffMins}m ago`;
    
    const diffHours = Math.floor(diffMins / 60);
    if (diffHours < 24) return `${diffHours}h ago`;
    
    return date.toLocaleDateString();
  };

  if (!game) {
    return <div>Loading...</div>;
  }

  return (
    <div className="flex flex-col h-full max-h-[70vh]">
      {/* Messages container */}
      <div 
        ref={messagesContainerRef}
        className="flex-1 overflow-y-auto space-y-3 mb-4 p-4 bg-gray-50 dark:bg-gray-800 rounded-lg"
      >
        {hasMore && messages.length > 0 && (
          <button
            onClick={handleLoadOlder}
            disabled={fetching}
            className="w-full py-2 text-sm text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-gray-200"
          >
            {fetching ? 'Loading...' : 'Load older messages'}
          </button>
        )}

        {messages.length === 0 && !fetching && (
          <div className="text-center text-gray-500 dark:text-gray-400 py-8">
            No messages yet. Be the first to say something!
          </div>
        )}

        {messages.map((message) => {
          const isOwnMessage = message.userId === currentPlayerId;
          const quoted = message.quotedMessageId ? getQuotedMessage(message.quotedMessageId) : null;

          return (
            <div
              key={message.messageId}
              className={`flex ${isOwnMessage ? 'justify-end' : 'justify-start'}`}
            >
              <div className={`max-w-[70%] ${isOwnMessage ? 'items-end' : 'items-start'} flex flex-col`}>
                {/* Sender name and timestamp */}
                <div className="text-xs text-gray-600 dark:text-gray-400 mb-1 px-2">
                  {getPlayerName(message.userId)} · {formatTimestamp(message.timestamp)}
                </div>

                {/* Message bubble */}
                <div
                  className={`rounded-lg px-4 py-2 ${
                    isOwnMessage
                      ? 'bg-blue-500 text-white'
                      : 'bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100'
                  }`}
                >
                  {/* Quoted message */}
                  {quoted && (
                    <div className="mb-2 pb-2 border-l-2 border-gray-400 pl-2 text-xs opacity-75">
                      <div className="font-semibold">{getPlayerName(quoted.userId)}</div>
                      <div className="italic">{truncateQuote(quoted.content)}</div>
                    </div>
                  )}

                  {/* Message content */}
                  <div className="whitespace-pre-wrap break-words">{message.content}</div>
                </div>

                {/* Reply button */}
                <button
                  onClick={() => handleReply(message.messageId)}
                  className="text-xs text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300 mt-1 px-2"
                  disabled={isChatDisabled}
                >
                  Reply
                </button>
              </div>
            </div>
          );
        })}
        <div ref={messagesEndRef} />
      </div>

      {/* Input area */}
      <div className="border-t border-gray-300 dark:border-gray-600 pt-4">
        {quotedMessage && (
          <div className="mb-2 p-2 bg-gray-100 dark:bg-gray-700 rounded flex justify-between items-center">
            <div className="text-sm text-gray-600 dark:text-gray-300">
              Replying to: {truncateQuote(getQuotedMessage(quotedMessage)?.content || '', 50)}
            </div>
            <button
              onClick={() => setQuotedMessage(null)}
              className="text-red-500 hover:text-red-700 text-sm"
            >
              Cancel
            </button>
          </div>
        )}

        <form onSubmit={handleSubmit} className="flex gap-2">
          <input
            type="text"
            value={messageInput}
            onChange={(e) => setMessageInput(e.target.value)}
            placeholder={isChatDisabled ? 'Chat is disabled' : 'Type a message (max 140 chars)...'}
            disabled={isChatDisabled}
            maxLength={140}
            className="flex-1 px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 disabled:opacity-50"
          />
          <button
            type="submit"
            disabled={isChatDisabled || !messageInput.trim()}
            className="px-6 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Send
          </button>
        </form>
        <div className="text-xs text-gray-500 dark:text-gray-400 mt-1 text-right">
          {messageInput.length}/140
        </div>
      </div>
    </div>
  );
};

export default ChatTab;
