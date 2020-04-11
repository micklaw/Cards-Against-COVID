using Newtonsoft.Json;

namespace CardsAgainstHumanity.Application.Models.Api
{
    public class ShufflePlayerCardsRequest
    {
        [JsonProperty("playerId")]
        public int PlayerId { get; set; }
    }
}