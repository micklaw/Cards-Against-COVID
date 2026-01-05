# Chat Feature Implementation Summary

## Overview
This document summarizes the implementation of the chat feature for the Cards-Against-COVID game, following the comprehensive requirements specification.

## Architecture

### Backend (C# / Azure Functions)

#### Models & Entities
1. **ChatMessage.cs** (`Application/Models/Api/`)
   - Properties: messageId, gameId, userId, content, quotedMessageId, timestamp
   - Used for API responses and internal messaging

2. **Chat.cs** (`Api/Entities/`)
   - ActorTableEntity implementation
   - Storage container: "entitychat"
   - Methods:
     - `AddMessage()` - Adds new message with GUID-based ID
     - `GetMessages()` - Pagination with before/after cursors

3. **Game.cs** (Updated)
   - Added `IsChatEnabled` property (defaults to false)
   - Added `SetChatEnabled()` method
   - Increments version on change for polling

#### Services
1. **IChatService / ChatService** (`Application/Services/`)
   - `SanitizeContent()` - Removes HTML tags, trims whitespace
   - `ValidateMessage()` - Checks length <= 140 chars

#### API Endpoints
All routes prefixed with `/game/{instance}/chat`

1. **GET /messages**
   - Query params: `beforeMessageId`, `afterMessageId`, `limit` (default 20, max 100)
   - Returns: `ChatMessage[]`
   - Authorization: Anonymous
   - Use case: Initial load, pagination, polling for new messages

2. **POST /messages**
   - Body: `{ userId, content, quotedMessageId? }`
   - Returns: `ChatMessage` (newly created)
   - Validation: 
     - Content sanitization
     - 140 char limit
     - Chat must be enabled
     - Game must not be over
   - Authorization: Anonymous

3. **PUT /settings**
   - Body: `{ isChatEnabled }`
   - Returns: Updated `Game` object
   - Use case: Game owner toggling chat on/off
   - Authorization: Anonymous (relies on client-side owner check)

### Frontend (React / TypeScript)

#### Type Definitions
- Added `ChatMessage` interface to `game.ts`
- Added `Tab.Chat` to tab enum
- Added `isChatEnabled` to `Game` interface

#### API Client (`gameApi.ts`)
- `getChatMessages()` - Fetch with pagination params
- `postChatMessage()` - Send new message
- `setChatSettings()` - Toggle chat enabled

#### Redux State Management

1. **chatSlice.ts**
   - State: messages, fetching, error, hasMore
   - Async thunks:
     - `fetchInitialMessages` - Load last 20 on mount
     - `fetchOlderMessages` - Pagination (load previous 20)
     - `fetchNewerMessages` - Polling (get new messages)
     - `postMessage` - Send message
   - Optimizations:
     - Set-based deduplication for O(1) performance
     - Maintains chronological order

2. **gameSlice.ts** (Updated)
   - Added `setChatSettings` async thunk
   - Updates game state including isChatEnabled

#### Components

1. **ChatTab.tsx**
   - WhatsApp-like UI design
   - Features:
     - Message bubbles with sender name and timestamp
     - Auto-scroll to latest message
     - Reply functionality with quote display
     - Character counter (140 limit)
     - "Load older messages" button
     - Polling every 5 seconds for new messages
   - Optimizations:
     - Message Map for O(1) lookups (memoized)
     - Efficient quoted message rendering
   - Disabled states:
     - Chat not enabled
     - Game is over
     - User not in game

2. **GameTabs.tsx** (Updated)
   - Added Chat tab to navigation
   - Unread badge implementation:
     - Red dot indicator
     - Cookie-based tracking (last visit timestamp)
     - Disappears when tab is active
     - Reappears for new messages
   - Memoized unread calculation

3. **StatsTab.tsx** (Updated)
   - Added "Chat On/Off" toggle button for game owners
   - Button styling reflects current state
   - Only visible to game creator

#### Utilities (`cookies.ts`)
- `setChatVisitCookie()` - Stores last visit timestamp
- `getChatVisitCookie()` - Retrieves last visit for unread calculation
- Cookie expiry: 7 days

## Technical Decisions

### Message ID Generation
- Format: `{yyyyMMddHHmmss}-{GUID}`
- Ensures uniqueness in concurrent scenarios
- Sortable by timestamp prefix

### Storage Pattern
- Uses ActorTableEntity for transaction safety
- Entity locking prevents race conditions
- Blobs stored in "entitychat" container
- Messages stored as list within entity

### Pagination Strategy
- Cursor-based (messageId) rather than offset
- Supports bidirectional loading (older/newer)
- Default limit: 20 messages
- No auto-load on scroll (explicit button click)

### Real-time Updates
- Long polling instead of WebSockets/SignalR
- 5-second poll interval (separate from game state polling)
- Polls for messages after last known messageId
- Deduplication on client side

### Security Considerations
- Server-side content sanitization (HTML tag removal)
- 140 character limit enforced
- No rate limiting (trust-based)
- No authentication beyond game membership
- Authorization relies on client-side checks (suitable for casual game)

### Performance Optimizations
- Map/Set data structures for O(1) lookups
- Memoized message map in ChatTab
- Efficient deduplication algorithm
- Conditional rendering for large lists
- Auto-scroll only on new messages

## File Structure

```
api/
├── CardsAgainstHumanity.Api/
│   ├── Entities/
│   │   ├── Chat.cs (NEW)
│   │   └── Game.cs (UPDATED)
│   ├── FunctionTriggers.cs (UPDATED - 3 new endpoints)
│   └── Program.cs (UPDATED - register ChatService)
├── CardsAgainstHumanity.Application/
│   ├── Interfaces/
│   │   └── IChatService.cs (NEW)
│   ├── Models/
│   │   ├── Api/
│   │   │   └── ChatMessage.cs (NEW)
│   │   └── Requests/
│   │       ├── PostChatMessageRequest.cs (NEW)
│   │       └── ChatSettingsRequest.cs (NEW)
│   └── Services/
│       └── ChatService.cs (NEW)

web/
├── src/
│   ├── api/
│   │   └── gameApi.ts (UPDATED - 3 new methods)
│   ├── components/
│   │   ├── ChatTab.tsx (NEW)
│   │   ├── GameTabs.tsx (UPDATED - chat tab + badge)
│   │   └── StatsTab.tsx (UPDATED - chat toggle)
│   ├── store/
│   │   ├── chatSlice.ts (NEW)
│   │   ├── gameSlice.ts (UPDATED - setChatSettings)
│   │   └── index.ts (UPDATED - register chat reducer)
│   ├── types/
│   │   └── game.ts (UPDATED - ChatMessage + Tab.Chat)
│   └── utils/
│       └── cookies.ts (UPDATED - chat visit tracking)

CHAT_TESTING.md (NEW)
CHAT_IMPLEMENTATION.md (NEW)
```

## Compliance with Requirements

### ✅ Completed Features
1. Chat Tab with Conversational UI
   - WhatsApp-like message bubbles
   - Sender name and timestamp
   - Reply with quoting (truncated, nested stripped)

2. Message Input
   - 140 character limit with counter
   - Disabled when game closed or chat disabled
   - Text-only, sanitized

3. Unread Message Badge
   - Dot indicator on tab
   - Cookie-based tracking
   - Disappears on visit

4. Message Fetching
   - Initial 20 messages
   - Pagination (load older)
   - Polling for new (5s interval)
   - Messages preserved when disabled

5. Chat Visibility Toggle
   - Game owner control
   - Default disabled
   - Disables input but keeps history

### Backend Requirements ✅
1. Storage in entitychat (ActorTableEntity)
2. All required fields in ChatMessage
3. GET/POST/PUT endpoints implemented
4. beforeMessageId/afterMessageId pagination
5. Cookie-based unread (client-side)
6. Server-side validation and sanitization

### Additional Considerations
- No explicit mark-as-read endpoint (cookie-based)
- Polling integrated with app's existing pattern
- Scalable architecture
- Efficient queries and rendering

## Future Enhancements (Out of Scope)
- Message deletion/editing
- Reactions/emoji support
- Typing indicators
- Read receipts
- File attachments
- Message search
- Push notifications
- Rate limiting
- User blocking/muting
- Message history archival
- Analytics/monitoring

## Deployment Notes
- No database migrations needed (ActorTableEntity handles schema)
- No environment variable changes required
- Azure Table Storage used (existing connection)
- Build and deployment via existing CI/CD
- Feature can be gradually rolled out

## Testing Status
- ✅ Backend builds successfully
- ✅ Frontend builds successfully  
- ✅ Linting passes (except pre-existing issue)
- ✅ Code review completed and addressed
- ⏳ Manual testing pending (see CHAT_TESTING.md)
- ⏳ Integration testing pending
- ⏳ Performance testing pending

## Metrics & Monitoring
Suggested metrics to track post-deployment:
- Chat enable/disable frequency
- Messages sent per game
- Average message length
- Reply usage rate
- Pagination usage
- Polling efficiency
- Error rates (validation failures)
- Storage costs (entitychat blob growth)
