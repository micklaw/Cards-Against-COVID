using System.Threading.Tasks;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class NextRoundEffect : Effect<NextRoundAction>
    {
        private readonly IApiClient apiClient;

        public NextRoundEffect(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        protected override async Task HandleAsync(NextRoundAction action, IDispatcher dispatcher)
        {
            var game = await this.apiClient.NextRound(action.InstanceName);

            if (game != null)
            {
                dispatcher.Dispatch(new UpdateGameStateAction(game));
            }
        }
    }
}
