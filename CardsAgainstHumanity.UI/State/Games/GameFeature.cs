using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games
{
    public class GameFeature : Feature<GameState>
    {
        public override string GetName() => "Game";

        protected override GameState GetInitialState() => new GameState();
    }
}
