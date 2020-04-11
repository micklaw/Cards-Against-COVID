using System;
using System.Collections.Generic;
using System.Text;

namespace CardsAgainstHumanity.Api.Models
{
    public class AddPlayerModel
    {
        public string PlayerName { get; set; }

        public IList<string> Responses { get; set; }
    }
}
