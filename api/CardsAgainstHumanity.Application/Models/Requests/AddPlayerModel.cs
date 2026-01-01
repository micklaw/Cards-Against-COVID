using System.Collections.Generic;

namespace CardsAgainstHumanity.Application.Models.Requests
{
    public class AddPlayerModel
    {
        public string PlayerName { get; set; }

        public IList<string> Responses { get; set; }
    }
}
