namespace CardsAgainstHumanity.UI.State.Games.Actions
{
    public class CreateGameAction : BaseGameAction
    {
        public string GameName { get; }

        public CreateGameAction(string gameName) : base(gameName)
        {
            this.GameName = gameName;
        }
    }
}
