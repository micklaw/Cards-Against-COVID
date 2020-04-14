using System.Collections.Generic;
using CardsAgainstHumanity.Application.Interfaces;

namespace CardsAgainstHumanity.UI.State.Games.Actions
{
    public class UpdateCurrentPlayersStateAction
    {
        public int? CurrentPlayerId { get; }

        public IGame Game { get; }

        public UpdateCurrentPlayersStateAction(IGame game, int? currentPlayerId)
        {
            this.Game = game;
            this.CurrentPlayerId = currentPlayerId;
        }
    }
}
