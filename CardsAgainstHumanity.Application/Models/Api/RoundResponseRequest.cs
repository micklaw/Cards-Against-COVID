using System.Collections.Generic;
using Newtonsoft.Json;

namespace CardsAgainstHumanity.Application.Models.Api
{
    public class RoundResponseRequest
    {
        [JsonProperty("playerId")]
        public int PlayerId { get; set; }

        [JsonProperty("responses")]
        public IList<string> Responses { get; set; }
    }
}