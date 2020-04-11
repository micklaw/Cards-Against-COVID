using Newtonsoft.Json;

namespace CardsAgainstHumanity.Application.Models.Api
{
    public class RoundVoteRequest
    {
        [JsonProperty("playerId")]
        public int PlayerId { get; set; }

        [JsonProperty("voteeId")]
        public int VoteeId { get; set; }
    }
}