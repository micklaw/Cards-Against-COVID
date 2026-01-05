namespace CardsAgainstHumanity.Application.Interfaces;

public interface IChatService
{
    string SanitizeContent(string content);
    bool ValidateMessage(string content);
}
