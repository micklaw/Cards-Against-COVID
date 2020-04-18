using System.Collections.Generic;

namespace CardsAgainstHumanity.Application.Models.Requests
{
    public class ShufflePlayerCardsModel
    {
        public int PlayerId { get; set; }

        public IList<string> Responses { get; set; }
    }
}
