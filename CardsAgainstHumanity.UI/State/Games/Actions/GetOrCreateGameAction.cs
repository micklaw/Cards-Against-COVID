namespace CardsAgainstHumanity.UI.State.Games.Actions
{
    public class GetOrCreateGameAction : BaseGameAction
    {
        public string GameName { get; }

        public GetOrCreateGameAction(string gameName) : base(gameName)
        {
            this.GameName = gameName;
        }
    }
}
