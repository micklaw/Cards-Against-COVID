using System.Threading.Tasks;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class NextRoundEffect : CommonUpdateGameEffect<NextRoundAction>
    {
        private readonly IApiClient apiClient;

        public NextRoundEffect(IApiClient apiClient) : base(apiClient)
        {
            this.apiClient = apiClient;
        }

        public override async Task HandleAsync(NextRoundAction action, IDispatcher dispatcher)
        {
            await this.apiClient.NextRound(action.InstanceName);
            await this.TryUpdateGame(action, dispatcher);
        }
    }
}
