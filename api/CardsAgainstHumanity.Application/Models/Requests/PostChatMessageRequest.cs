namespace CardsAgainstHumanity.Application.Models.Requests;

public class PostChatMessageRequest
{
    public int UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? QuotedMessageId { get; set; }
}
