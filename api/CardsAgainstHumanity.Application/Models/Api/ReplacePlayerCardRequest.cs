using Newtonsoft.Json;

namespace CardsAgainstHumanity.Application.Models.Api
{
    public class ReplacePlayerCardRequest
    {
        [JsonProperty("playerId")]
        public int PlayerId { get; set; }

        [JsonProperty("cardIndex")]
        public int CardIndex { get; set; }

        public string Response { get; set; }
    }
}