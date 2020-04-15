using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games
{
    public class ConnectionFeature : Feature<ConnectionState>
    {
        public override string GetName() => "Connection";

        protected override ConnectionState GetInitialState() => new ConnectionState();
    }
}
