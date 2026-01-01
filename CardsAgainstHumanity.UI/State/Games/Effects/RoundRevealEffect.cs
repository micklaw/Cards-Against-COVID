using System.Threading.Tasks;
using CardsAgainstHumanity.Application.Models.Api;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class RoundRevealEffect : CommonUpdateGameEffect<RoundRevealAction>
    {
        private readonly IApiClient apiClient;

        public RoundRevealEffect(IApiClient apiClient) : base(apiClient)
        {
            this.apiClient = apiClient;
        }

        public override async Task HandleAsync(RoundRevealAction action, IDispatcher dispatcher)
        {
            await this.apiClient.Reveal(action.InstanceName);
            await this.TryUpdateGame(action, dispatcher);
        }
    }
}
