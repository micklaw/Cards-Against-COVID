using System.Threading.Tasks;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class CloseGameEffect : Effect<CloseGameAction>
    {
        private readonly IApiClient apiClient;

        public CloseGameEffect(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        protected override async Task HandleAsync(CloseGameAction action, IDispatcher dispatcher)
        {
            var game = await this.apiClient.Close(action.InstanceName);

            if (game != null)
            {
                dispatcher.Dispatch(new UpdateGameStateAction(game));
            }
        }
    }
}
