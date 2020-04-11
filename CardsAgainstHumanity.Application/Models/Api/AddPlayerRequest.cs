using Newtonsoft.Json;

namespace CardsAgainstHumanity.Application.Models.Api
{
    public class AddPlayerRequest
    {
        [JsonProperty("playerName")]
        public string PlayerName { get; set; }
    }
}