using CardsAgainstHumanity.Application.Interfaces;

namespace CardsAgainstHumanity.UI.State.Games.Actions
{
    public class UpdateGameStateAction
    {
        public IGame Game { get; }

        public UpdateGameStateAction(IGame game)
        {
            this.Game = game;
        }
    }
}
