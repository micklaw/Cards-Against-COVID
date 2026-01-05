using CardsAgainstHumanity.Application.Models.Api;

namespace CardsAgainstHumanity.Application.Interfaces;

public interface IChatService
{
    string SanitizeContent(string content);
    bool ValidateMessage(string content);
}
