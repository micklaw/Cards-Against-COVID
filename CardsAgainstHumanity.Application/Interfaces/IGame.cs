using System;
using System.Collections.Generic;
using System.Text;
using CardsAgainstHumanity.Application.Models;

namespace CardsAgainstHumanity.Application.Interfaces
{
    public interface IGame
    {
        string Url { get; set; }

        string Name { get; set; }

        string Code { get; set; }

        int CardCount { get; set; }

        IList<Player> Players { get; set; }

        Round CurrentRound { get; set; }

        IList<Round> PreviousRounds { get; set; }

        bool IsOpen { get; set; }

        bool IsOver { get; set; }
    }
}
