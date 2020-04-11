using CardsAgainstHumanity.Application.Interfaces;
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

        public Tab CurrentTab { get; set; } = Tab.Stats;
    }
}
