using System.Collections.Generic;
using System.Linq;
using CardsAgainstHumanity.Application.Interfaces;
using CardsAgainstHumanity.Application.Models;
using CardsAgainstHumanity.UI.State.Games.Models;

namespace CardsAgainstHumanity.UI.State.Games
{
    public enum Tab
    {
        Stats,
        Round,
        Cards
    }

    public class GameState
    {
        public Game Game { get; set; }

        public Dictionary<int, int> VoteTable => Game?.CurrentRound?.Votes
                                    ?.GroupBy(i => i)
                                    ?.OrderByDescending(i => (i?.Count() ?? 0))
                                    ?.ToDictionary(i => i.Key, i => (i?.Count() ?? 0)) ?? new Dictionary<int, int>();

        public Player CurrentPlayer => Game?.Players?.FirstOrDefault();

        public int RoundCount()
        {
            var previousCount = Game.PreviousRounds?.Count ?? 0;

            if (Game.CurrentRound != null)
            {
                previousCount++;
            }

            return previousCount;
        }

        public string GetPlayerName(int playerId, bool useIsWon = false)
        {
            if (Game?.Players == null || !Game.Players.Any() || playerId < 0)
            {
                return "Unknown";
            }

            return (!useIsWon ? 
                Game.Players.FirstOrDefault(i => i.Id == playerId)?.Name :
                    Game.CurrentRound.IsWon ? 
                        Game.Players.FirstOrDefault(i => i.Id == playerId)?.Name :
                            "******") ?? "Unknown";
        }

        public int GetCurrentRoundPlayerVotes(int playerId)
        {
            if (VoteTable == null)
            {
                return 0;
            }

            if (VoteTable.ContainsKey(playerId))
            {
                return VoteTable[playerId];
            }

            return 0;
        }
    }
}
