using System.Collections;
using System.Collections.Generic;

namespace CardsAgainstHumanity.UI.State.Games.Actions
{
    public class RoundRespondAction : BaseGameAction
    {
        public int PlayerId { get; }

        public IList<int> Responses { get; }

        public RoundRespondAction(string gameName, int playerId, IList<int> responses) : base(gameName)
        {
            PlayerId = playerId;
            Responses = responses;
        }
    }
}
