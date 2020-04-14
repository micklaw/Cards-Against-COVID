using System.Collections.Generic;
using System.Linq;
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

        public bool PartOfCurrentGame => CurrentPlayer != null;

        public Dictionary<int, int> VoteTable { get; set; }

        public Player CurrentPlayer { get; set; }

        public int RoundCount { get; set; }

        public IList<int> CurrentResponses { get; set; }

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
