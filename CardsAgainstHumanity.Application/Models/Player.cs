using System.Collections.Generic;
using System.Linq;

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

        public void Replace(int cardIndex, string response)
        {
            var card = Cards?.Select((item, index) => new
                {
                    Item = item,
                    Index = index
                })
                .FirstOrDefault(i => i.Index == cardIndex)
                ?.Item;

            if (card != null)
            {
                Cards.Insert(cardIndex, response);
                Cards.Remove(card);
            }
        }
    }
}
