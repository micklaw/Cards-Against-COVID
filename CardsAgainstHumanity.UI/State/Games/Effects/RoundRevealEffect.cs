using System.Threading.Tasks;
using CardsAgainstHumanity.Application.Models.Api;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class RoundRevealEffect : Effect<RoundRevealAction>
    {
        private readonly IApiClient apiClient;

        public RoundRevealEffect(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        protected override async Task HandleAsync(RoundRevealAction action, IDispatcher dispatcher)
        {
            var game = await this.apiClient.Reveal(action.InstanceName);

            if (game != null)
            {
                dispatcher.Dispatch(new UpdateGameStateAction(game));
            }
        }
    }
}
