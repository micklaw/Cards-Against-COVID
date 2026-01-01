using System.Threading.Tasks;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class FinishGameEffect : CommonUpdateGameEffect<FinishGameAction>
    {
        private readonly IApiClient apiClient;

        public FinishGameEffect(IApiClient apiClient) : base(apiClient)
        {
            this.apiClient = apiClient;
        }

        public override async Task HandleAsync(FinishGameAction action, IDispatcher dispatcher)
        {
            await this.apiClient.Finish(action.InstanceName);
            await this.TryUpdateGame(action, dispatcher);
        }
    }
}
