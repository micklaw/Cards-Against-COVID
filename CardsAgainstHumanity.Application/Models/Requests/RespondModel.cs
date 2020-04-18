using System.Collections.Generic;

namespace CardsAgainstHumanity.Application.Models.Requests
{
    public class RespondModel
    {
        public int PlayerId { get; set; }

        public List<int> Responses { get; set; }
    }
}
