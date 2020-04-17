using System.Threading.Tasks;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class NewRoundPromptEffect : CommonUpdateGameEffect<NewRoundPromptAction>
    {
        private readonly IApiClient apiClient;

        public NewRoundPromptEffect(IApiClient apiClient) : base(apiClient)
        {
            this.apiClient = apiClient;
        }

        protected override async Task HandleAsync(NewRoundPromptAction action, IDispatcher dispatcher)
        {
            await this.apiClient.NewPrompt(action.InstanceName);
            await this.TryUpdateGame(action, dispatcher);
        }
    }
}
