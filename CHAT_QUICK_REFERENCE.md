# Chat Feature - Quick Reference

## 🎯 Quick Start

### For Game Owners
1. Create a game
2. Go to **Stats** tab
3. Click **"💬 Chat Off"** button to enable chat
4. Players can now use the **Chat** tab

### For Players
1. Join a game
2. Wait for owner to enable chat
3. Click **Chat** tab
4. Type messages (max 140 chars)
5. Click **Reply** to quote messages

## 📊 Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                         Frontend (React)                     │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │  GameTabs    │  │   ChatTab    │  │  StatsTab    │      │
│  │  Component   │  │  Component   │  │  Component   │      │
│  ├──────────────┤  ├──────────────┤  ├──────────────┤      │
│  │ - Chat Badge │  │ - Messages   │  │ - Chat Toggle│      │
│  │ - Unread     │  │ - Input      │  │   Button     │      │
│  │   Tracking   │  │ - Pagination │  │              │      │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘      │
│         │                 │                  │               │
│         └─────────────────┼──────────────────┘               │
│                           │                                  │
│                    ┌──────▼───────┐                          │
│                    │  Redux Store │                          │
│                    ├──────────────┤                          │
│                    │ - gameSlice  │                          │
│                    │ - chatSlice  │                          │
│                    └──────┬───────┘                          │
│                           │                                  │
│                    ┌──────▼───────┐                          │
│                    │   gameApi    │                          │
│                    └──────┬───────┘                          │
└───────────────────────────┼──────────────────────────────────┘
                            │
                            │ HTTP/REST
                            │
┌───────────────────────────▼──────────────────────────────────┐
│                    Backend (Azure Functions)                  │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│                  ┌───────────────────┐                        │
│                  │ FunctionTriggers  │                        │
│                  ├───────────────────┤                        │
│                  │ GET /chat/messages│                        │
│                  │ POST /chat/messages│                       │
│                  │ PUT /chat/settings│                        │
│                  └─────────┬─────────┘                        │
│                            │                                  │
│              ┌─────────────┼─────────────┐                   │
│              │             │             │                    │
│      ┌───────▼──────┐ ┌───▼────┐ ┌─────▼─────┐             │
│      │ ChatService  │ │  Game  │ │   Chat    │             │
│      │  (validate)  │ │ Entity │ │  Entity   │             │
│      └──────────────┘ └───┬────┘ └─────┬─────┘             │
│                           │            │                     │
│                           └────────┬───┘                     │
│                                    │                         │
│                        ┌───────────▼──────────┐             │
│                        │ ActorTableEntity     │             │
│                        │      Client          │             │
│                        └───────────┬──────────┘             │
└────────────────────────────────────┼──────────────────────────┘
                                     │
                                     │
                          ┌──────────▼──────────┐
                          │  Azure Table Storage│
                          ├─────────────────────┤
                          │  entitygame         │
                          │  entitychat         │
                          └─────────────────────┘
```

## 🔄 Message Flow

### Sending a Message
```
User Types → ChatTab → postMessage()
    ↓
Redux chatSlice → gameApi.postChatMessage()
    ↓
POST /chat/messages → FunctionTriggers
    ↓
ChatService.Sanitize → Chat.AddMessage()
    ↓
ActorTableEntity.Flush → Azure Table Storage
    ↓
Response → Redux → ChatTab (renders new message)
```

### Receiving Messages (Polling)
```
Timer (5s) → fetchNewerMessages()
    ↓
GET /chat/messages?afterMessageId=X
    ↓
FunctionTriggers → Chat.GetMessages()
    ↓
Azure Table Storage → Response
    ↓
Redux (deduplicate) → ChatTab (render)
```

### Toggling Chat
```
Owner Clicks → StatsTab → setChatSettings()
    ↓
PUT /chat/settings → FunctionTriggers
    ↓
Game.SetChatEnabled() → Increment Version
    ↓
Azure Table Storage → Response
    ↓
Redux gameSlice → All Components Update
```

## 📝 Key Files

### Backend
- `api/CardsAgainstHumanity.Api/Entities/Chat.cs` - Chat entity
- `api/CardsAgainstHumanity.Api/FunctionTriggers.cs` - API endpoints
- `api/CardsAgainstHumanity.Application/Services/ChatService.cs` - Validation

### Frontend
- `web/src/components/ChatTab.tsx` - Main chat UI
- `web/src/store/chatSlice.ts` - Redux state
- `web/src/api/gameApi.ts` - API client

### Documentation
- `CHAT_TESTING.md` - Testing guide
- `CHAT_IMPLEMENTATION.md` - Implementation details

## 🚀 API Endpoints

### GET `/game/{instance}/chat/messages`
Fetch messages with pagination.

**Query Parameters:**
- `beforeMessageId` (optional) - Get messages before this ID
- `afterMessageId` (optional) - Get messages after this ID  
- `limit` (optional) - Number of messages (default: 20, max: 100)

**Response:** `ChatMessage[]`

### POST `/game/{instance}/chat/messages`
Send a new message.

**Body:**
```json
{
  "userId": 1,
  "content": "Hello, world!",
  "quotedMessageId": "20260105120000-abc123" // optional
}
```

**Response:** `ChatMessage`

### PUT `/game/{instance}/chat/settings`
Toggle chat enabled/disabled (game owner only).

**Body:**
```json
{
  "isChatEnabled": true
}
```

**Response:** Updated `Game` object

## 🎨 UI Components

### Chat Tab
- Message bubbles (blue for self, white for others)
- Sender name and timestamp
- Reply button with quote display
- Input with 140 char counter
- "Load older messages" button
- Auto-scroll to latest

### Unread Badge
- Red dot on Chat tab
- Cookie-based tracking
- Disappears when visited
- Reappears for new messages

### Chat Toggle
- In Stats tab (owner only)
- "💬 Chat On/Off" button
- Green when enabled, gray when disabled

## 🔒 Security & Validation

- ✅ Server-side HTML sanitization
- ✅ 140 character limit enforced
- ✅ Plaintext only (no HTML/scripts)
- ✅ Chat disabled check on POST
- ✅ Game-over check on POST
- ⚠️ No rate limiting (trust-based)
- ⚠️ No authentication (game membership only)

## ⚡ Performance

- **O(1) message lookups** using Map
- **Efficient deduplication** using Set
- **Memoized components** for fast rendering
- **Cursor-based pagination** for scalability
- **5-second polling** interval
- **20 messages per page** default

## 📦 Storage

- **Container:** `entitychat` (Azure Table Storage)
- **Entity:** One per game
- **Message ID Format:** `{yyyyMMddHHmmss}-{GUID}`
- **Retention:** No automatic cleanup
- **Size:** ~200 bytes per message

## 🐛 Known Limitations

- No message editing or deletion
- No typing indicators
- No read receipts
- No file attachments
- No emoji reactions
- No user blocking
- No message search

## 📚 Further Reading

- [CHAT_TESTING.md](./CHAT_TESTING.md) - Manual testing guide
- [CHAT_IMPLEMENTATION.md](./CHAT_IMPLEMENTATION.md) - Technical details
- [README.md](./README.md) - Project overview
