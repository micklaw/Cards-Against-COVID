using Newtonsoft.Json;

namespace CardsAgainstHumanity.Application.Models.Api
{
    public class GetOrCreateRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}