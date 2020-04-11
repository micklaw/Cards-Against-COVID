using System.Collections.Generic;

namespace CardsAgainstHumanity.Application.Models
{
    public class Player
    {
        public Player(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public IList<string> Cards { get; set; } = new List<string>();

        public void Shuffle(IList<string> responses)
        {
            Cards = responses;
        }
    }
}
