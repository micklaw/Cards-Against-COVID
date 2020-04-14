namespace CardsAgainstHumanity.UI.State.Games.Actions
{
    public class ReplacePlayerCardAction : BaseGameAction
    {
        public int PlayerId { get; }

        public int CardIndex { get; }

        public ReplacePlayerCardAction(string gameName, int playerId, int cardIndex) : base(gameName)
        {
            PlayerId = playerId;
            CardIndex = cardIndex;
        }
    }
}
