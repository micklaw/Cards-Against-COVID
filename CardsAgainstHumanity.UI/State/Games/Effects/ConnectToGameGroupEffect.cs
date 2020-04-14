using System.Threading.Tasks;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class ConnectToGameGroupEffect : Effect<ConnectToGameGroupAction>
    {
        private readonly IApiClient apiClient;

        public ConnectToGameGroupEffect(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        protected override Task HandleAsync(ConnectToGameGroupAction action, IDispatcher dispatcher)
        {
            // TODO: Connect to Signalr group with instance name

            return Task.CompletedTask;
        }
    }
}
