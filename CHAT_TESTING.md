# Chat Feature Manual Testing Guide

This document provides comprehensive testing instructions for the new chat feature in Cards-Against-COVID.

## Prerequisites
- Backend running on http://localhost:7071
- Frontend running on http://localhost:4280 (via SWA CLI)
- At least 2 browser instances/tabs or incognito mode for multi-user testing

## Test Cases

### 1. Chat Settings - Default State
**Steps:**
1. Create a new game
2. Navigate to Stats tab
3. Check the "Chat Off" button is displayed
4. Navigate to Chat tab
5. Try to send a message

**Expected:**
- Chat is disabled by default (button shows "Chat Off")
- Chat tab exists but input is disabled
- Message shows "Chat is disabled"

### 2. Enable Chat (Game Owner)
**Steps:**
1. As game creator, go to Stats tab
2. Click "Chat Off" button
3. Navigate to Chat tab
4. Input field should be enabled

**Expected:**
- Button changes to "Chat On" with success styling
- Game version increments
- Input field becomes active
- Placeholder shows "Type a message (max 140 chars)..."

### 3. Send Messages
**Steps:**
1. With chat enabled, type "Hello, world!"
2. Click Send button
3. Type "This is a test message"
4. Press Enter key

**Expected:**
- Messages appear in chat with:
  - Sender name (your player name)
  - Timestamp (e.g., "Just now", "5m ago")
  - Message in a bubble (blue for own messages)
  - Right-aligned for own messages

### 4. Character Limit Validation
**Steps:**
1. Type exactly 140 characters in the input
2. Try to type more
3. Check character counter
4. Send message

**Expected:**
- Input allows exactly 140 characters, no more
- Counter shows "140/140"
- Message sends successfully
- Attempting to send >140 chars results in truncation/error

### 5. Reply Functionality
**Steps:**
1. Click "Reply" button under a message
2. Type a response
3. Send the message
4. Check the replied message

**Expected:**
- Quote preview appears above input showing original message
- Quote is truncated if >100 chars
- Sent message shows quoted text with indentation
- Nested quotes are stripped

### 6. Message Pagination
**Steps:**
1. Send 25+ messages in chat
2. Scroll to top of chat
3. Click "Load older messages"

**Expected:**
- Initial load shows last 20 messages
- "Load older messages" button appears
- Clicking loads previous 20 messages
- Messages maintain chronological order
- Scroll position preserved

### 7. Real-time Updates (Multi-user)
**Steps:**
1. Open game in 2 different browsers/tabs
2. Enable chat as game owner
3. Send message from user A
4. Wait ~5 seconds
5. Check user B's chat

**Expected:**
- Message appears in user B's chat automatically
- Poll interval is ~5 seconds
- No manual refresh needed
- Messages appear in correct order

### 8. Unread Badge
**Steps:**
1. User A sends 3 messages
2. User B stays on Stats tab
3. Check Chat tab for badge
4. Click Chat tab
5. Check badge again

**Expected:**
- Red dot appears on Chat tab when new messages exist
- Dot disappears when Chat tab is visited
- Cookie tracks last visit timestamp
- Badge reappears when new messages arrive after visit

### 9. Disable Chat
**Steps:**
1. Game owner clicks "Chat On" button
2. Non-owner users check their chat input
3. Try to send a message

**Expected:**
- Button changes to "Chat Off"
- All users' inputs become disabled
- Existing messages remain visible
- New messages cannot be sent
- Error message if attempting to POST message

### 10. Game State Interactions
**Steps:**
1. Enable chat
2. Close the game (as owner)
3. Check chat input
4. Finish the game
5. Check chat tab visibility

**Expected:**
- Closing game disables chat input
- Messages still visible
- Finishing game keeps chat visible
- Chat settings persist through game state changes

### 11. API Endpoint Testing

#### GET /game/{instance}/chat/messages
```bash
curl "http://localhost:7071/game/test-game/chat/messages?limit=20"
```
**Expected:** Returns array of 0-20 messages

#### POST /game/{instance}/chat/messages
```bash
curl -X POST "http://localhost:7071/game/test-game/chat/messages" \
  -H "Content-Type: application/json" \
  -d '{"userId": 1, "content": "Test message", "quotedMessageId": null}'
```
**Expected:** Returns newly created message with messageId and timestamp

#### PUT /game/{instance}/chat/settings
```bash
curl -X PUT "http://localhost:7071/game/test-game/chat/settings" \
  -H "Content-Type: application/json" \
  -d '{"isChatEnabled": true}'
```
**Expected:** Returns updated game state with isChatEnabled=true

### 12. Error Handling
**Steps:**
1. Try sending empty message
2. Try sending message with HTML tags: `<script>alert('xss')</script>`
3. Disable chat and try POST via API
4. Send message to non-existent game

**Expected:**
- Empty messages not sent
- HTML tags stripped/sanitized
- POST returns 400 error when chat disabled
- Non-existent game returns 404

### 13. Cookie Persistence
**Steps:**
1. Visit chat tab
2. Close browser
3. Reopen game URL
4. Check unread badge

**Expected:**
- Visit timestamp cookie persists (7 days)
- Badge calculation uses stored timestamp
- No unread messages for pre-visit messages

### 14. Performance Testing
**Steps:**
1. Send 100+ messages
2. Scroll through message list
3. Send new message
4. Reply to old message
5. Load older messages

**Expected:**
- Smooth scrolling with many messages
- No lag when sending new messages
- O(1) message lookups using Map
- Efficient rendering with memoization

## Known Limitations
- No delete/edit message functionality
- No message search
- No file attachments
- No emoji/reactions support
- No typing indicators
- No online/offline status

## Storage Details
- Messages stored in Azure Table Storage (entitychat container)
- Game settings stored in Game entity (entitygame container)
- Client-side cookies for visit tracking
- No database cleanup/archival

## Success Criteria
All test cases pass with expected results and:
- ✅ No console errors
- ✅ No memory leaks
- ✅ Responsive UI
- ✅ Smooth animations
- ✅ Clear error messages
- ✅ Accessible UI elements
