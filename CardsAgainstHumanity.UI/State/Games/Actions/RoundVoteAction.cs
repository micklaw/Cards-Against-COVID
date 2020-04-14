using System.Collections;
using System.Collections.Generic;

namespace CardsAgainstHumanity.UI.State.Games.Actions
{
    public class RoundVoteAction : BaseGameAction
    {
        public int PlayerId { get; }

        public int VoteeId { get; }

        public RoundVoteAction(string gameName, int playerId, int voteeId) : base(gameName)
        {
            PlayerId = playerId;
            VoteeId = voteeId;
        }
    }
}
