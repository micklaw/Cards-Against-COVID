using System;
using System.Collections.Generic;
using System.Text;

namespace CardsAgainstHumanity.Api.Models
{
    public class ShufflePlayerCardsModel
    {
        public int PlayerId { get; set; }

        public IList<string> Responses { get; set; }
    }
}
