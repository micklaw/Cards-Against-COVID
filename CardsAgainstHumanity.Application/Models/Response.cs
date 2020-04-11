using System.Collections.Generic;
using System.Linq;

namespace CardsAgainstHumanity.Application.Models
{
    public class Response
    {
        public int PlayerId { get; set; }

        public IList<string> Responses { get; set; } = new List<string>();
    }
}
