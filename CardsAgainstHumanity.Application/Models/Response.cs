﻿using System.Collections.Generic;
using System.Linq;

namespace CardsAgainstHumanity.Application.Models
{
    public class Response
    {
        public int PlayerId { get; set; }

        public IList<int> Responses { get; set; } = new List<int>();
    }
}
