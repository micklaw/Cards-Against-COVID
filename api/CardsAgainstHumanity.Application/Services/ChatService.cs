using CardsAgainstHumanity.Application.Interfaces;
using System.Text.RegularExpressions;

namespace CardsAgainstHumanity.Application.Services;

public class ChatService : IChatService
{
    private const int MaxMessageLength = 140;

    public string SanitizeContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return string.Empty;
        }

        // Remove any HTML tags
        content = Regex.Replace(content, "<.*?>", string.Empty);
        
        // Trim whitespace
        content = content.Trim();
        
        // Truncate to max length
        if (content.Length > MaxMessageLength)
        {
            content = content.Substring(0, MaxMessageLength);
        }

        return content;
    }

    public bool ValidateMessage(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return false;
        }

        if (content.Length > MaxMessageLength)
        {
            return false;
        }

        return true;
    }
}
