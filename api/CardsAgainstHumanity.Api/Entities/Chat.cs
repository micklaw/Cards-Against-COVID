using ActorTableEntities;
using CardsAgainstHumanity.Application.Models.Api;
using System.Collections.Generic;
using System.Linq;

namespace CardsAgainstHumanity.Api.Entities
{
    public class Chat : ActorTableEntity
    {
        public Chat()
        {
        }

        public Chat(string gameId)
            : base("entitychat", gameId)
        {
            GameId = gameId;
        }

        public string GameId { get; set; } = string.Empty;

        [ActorTableEntityComplexProperty]
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

        public Chat AddMessage(int userId, string content, string? quotedMessageId)
        {
            // Generate unique message ID using GUID
            var timestamp = DateTime.UtcNow;
            var messageId = $"{timestamp:yyyyMMddHHmmss}-{Guid.NewGuid():N}";

            var message = new ChatMessage
            {
                MessageId = messageId,
                GameId = GameId,
                UserId = userId,
                Content = content,
                QuotedMessageId = quotedMessageId,
                Timestamp = timestamp
            };

            Messages.Add(message);
            return this;
        }

        public List<ChatMessage> GetMessages(string? beforeMessageId = null, string? afterMessageId = null, int limit = 20)
        {
            var query = Messages.AsEnumerable();

            if (!string.IsNullOrEmpty(beforeMessageId))
            {
                // Get messages before the specified message (older messages)
                var beforeMessage = Messages.FirstOrDefault(m => m.MessageId == beforeMessageId);
                if (beforeMessage != null)
                {
                    query = query.Where(m => m.Timestamp < beforeMessage.Timestamp);
                }
            }

            if (!string.IsNullOrEmpty(afterMessageId))
            {
                // Get messages after the specified message (newer messages)
                var afterMessage = Messages.FirstOrDefault(m => m.MessageId == afterMessageId);
                if (afterMessage != null)
                {
                    query = query.Where(m => m.Timestamp > afterMessage.Timestamp);
                }
            }

            // Order by timestamp descending and take limit
            return query
                .OrderByDescending(m => m.Timestamp)
                .Take(limit)
                .OrderBy(m => m.Timestamp) // Re-order chronologically for display
                .ToList();
        }
    }
}
