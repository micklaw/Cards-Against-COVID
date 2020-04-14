using System.Threading.Tasks;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class NewRoundEffect : Effect<NewRoundAction>
    {
        private readonly IApiClient apiClient;

        public NewRoundEffect(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        protected override async Task HandleAsync(NewRoundAction action, IDispatcher dispatcher)
        {
            var game = await this.apiClient.NewRound(action.InstanceName);

            if (game != null)
            {
                dispatcher.Dispatch(new UpdateGameStateAction(game));
            }
        }
    }
}
