using System.Threading.Tasks;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class CloseGameEffect : CommonUpdateGameEffect<CloseGameAction>
    {
        private readonly IApiClient apiClient;

        public CloseGameEffect(IApiClient apiClient) : base(apiClient)
        {
            this.apiClient = apiClient;
        }

        public override async Task HandleAsync(CloseGameAction action, IDispatcher dispatcher)
        {
            await this.apiClient.Close(action.InstanceName);
            await this.TryUpdateGame(action, dispatcher);
        }
    }
}
