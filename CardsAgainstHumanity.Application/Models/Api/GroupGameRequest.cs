using Newtonsoft.Json;

namespace CardsAgainstHumanity.Application.Models.Api
{
    public class GroupGameRequest
    {
        public string ConnectionId { get; set; }

        public string UserId { get; set; }
    }
}