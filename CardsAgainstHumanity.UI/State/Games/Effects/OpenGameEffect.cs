using System.Threading.Tasks;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class OpenGameEffect : CommonUpdateGameEffect<OpenGameAction>
    {
        private readonly IApiClient apiClient;

        public OpenGameEffect(IApiClient apiClient) : base(apiClient)
        {
            this.apiClient = apiClient;
        }

        protected override async Task HandleAsync(OpenGameAction action, IDispatcher dispatcher)
        {
            await this.apiClient.Open(action.InstanceName);
            await this.TryUpdateGame(action, dispatcher);
        }
    }
}
