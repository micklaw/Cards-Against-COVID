using System.Threading.Tasks;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class NewRoundPromptEffect : Effect<NewRoundPromptAction>
    {
        private readonly IApiClient apiClient;

        public NewRoundPromptEffect(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        protected override async Task HandleAsync(NewRoundPromptAction action, IDispatcher dispatcher)
        {
            var game = await this.apiClient.NewPrompt(action.InstanceName);

            if (game != null)
            {
                dispatcher.Dispatch(new UpdateGameStateAction(game));
            }
        }
    }
}
