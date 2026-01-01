using Newtonsoft.Json;

namespace CardsAgainstHumanity.Application.Models.Api
{
    public class CreateRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}