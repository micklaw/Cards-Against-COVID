using System.Threading.Tasks;
using CardsAgainstHumanity.Application.Extensions;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class NewRoundEffect : CommonUpdateGameEffect<NewRoundAction>
    {
        private readonly IApiClient apiClient;

        public NewRoundEffect(IApiClient apiClient) : base(apiClient)
        {
            this.apiClient = apiClient;
        }

        public override async Task HandleAsync(NewRoundAction action, IDispatcher dispatcher)
        {
            await this.apiClient.NewRound(action.InstanceName);
            await this.TryUpdateGame(action, dispatcher);
        }
    }
}
