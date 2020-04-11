using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardsAgainstHumanity.Application.Interfaces;
using CardsAgainstHumanity.Application.Models;

namespace CardsAgainstHumanity.UI.State.Games.Models
{
    public class Game : IGame
    {
        public string Url { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public int CardCount { get; set; } = 7;

        public IList<Player> Players { get; set; } = new List<Player>();

        public Round CurrentRound { get; set; }

        public IList<Round> PreviousRounds { get; set; } = new List<Round>();

        public bool IsOpen { get; set; } = true;

        public bool IsOver { get; set; }

        public Dictionary<int, int> Score => PreviousRounds?.GroupBy(i => i.WonBy).ToDictionary(i => Players.First(p => p.Id == i.Key).Id, i => i.Count());
    }
}
