using CardsAgainstHumanity.UI.State.Games;
using CardsAgainstHumanity.UI.State.Games.Actions;
using CardsAgainstHumanity.UI.State.Games.Models;
using Fluxor;

namespace CardsAgainstHumanity.UI.State
{
    public class Reducers
    {
        [ReducerMethod]
        public static GameState ReduceGameState(GameState state, UpdateGameStateAction action)
        {
            var game = new Game()
            {
                CardCount = action.Game.CardCount,
                Code = action.Game.Code,
                CurrentRound = action.Game.CurrentRound,
                IsOpen = action.Game.IsOpen,
                IsOver = action.Game.IsOver,
                Name = action.Game.Name,
                Players = action.Game.Players,
                PreviousRounds = action.Game.PreviousRounds,
                Url = action.Game.Url
            };

            return new GameState()
            {
                Game = game
            };
        }
    }
}
