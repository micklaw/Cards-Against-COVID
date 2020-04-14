namespace CardsAgainstHumanity.UI.State.Games.Actions
{
    public class PlayerShuffleCardsAction : BaseGameAction
    {
        public int PlayerId { get; }

        public PlayerShuffleCardsAction(string gameName, int playerId) : base(gameName)
        {
            PlayerId = playerId;
        }
    }
}
