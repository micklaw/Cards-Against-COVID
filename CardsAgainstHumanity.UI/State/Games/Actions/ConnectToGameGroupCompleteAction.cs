namespace CardsAgainstHumanity.UI.State.Games.Actions
{
    public class ConnectToGameGroupCompleteAction : BaseGameAction
    {
        public string ConnectionId { get; }

        public string UserId { get; }

        public bool Connected { get; }

        public ConnectToGameGroupCompleteAction(string gameName, string connectionId, string userId, bool connected) : base(gameName)
        {
            ConnectionId = connectionId;
            UserId = userId;
            Connected = connected;
        }
    }
}
