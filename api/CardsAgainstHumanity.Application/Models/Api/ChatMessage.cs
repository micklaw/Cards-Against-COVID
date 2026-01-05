namespace CardsAgainstHumanity.Application.Models.Api;

public class ChatMessage
{
    public string MessageId { get; set; } = string.Empty;
    public string GameId { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? QuotedMessageId { get; set; }
    public DateTime Timestamp { get; set; }
}
