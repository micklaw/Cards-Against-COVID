namespace CardsAgainstHumanity.UI.State.Games.Actions
{
    public class ConnectToGameGroupAction : BaseGameAction
    {
        public string ConnectionId { get; }

        public ConnectToGameGroupAction(string gameName, string connectionId) : base(gameName)
        {
            ConnectionId = connectionId;
        }
    }
}
