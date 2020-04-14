namespace CardsAgainstHumanity.UI.State.Games.Actions
{
    public class PlayerAddAction : BaseGameAction
    {
        public string PlayerName { get; }

        public PlayerAddAction(string gameName, string playerName) : base(gameName)
        {
            PlayerName = playerName;
        }
    }
}
