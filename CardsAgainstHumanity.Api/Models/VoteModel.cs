using System;
using System.Collections.Generic;
using System.Text;

namespace CardsAgainstHumanity.Api.Models
{
    public class VoteModel
    {
        public int PlayerId { get; set; }

        public int VoteeId { get; set; }
    }
}
