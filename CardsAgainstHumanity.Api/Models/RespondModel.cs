using System;
using System.Collections.Generic;
using System.Text;

namespace CardsAgainstHumanity.Api.Models
{
    public class RespondModel
    {
        public int PlayerId { get; set; }

        public List<int> Responses { get; set; }
    }
}
