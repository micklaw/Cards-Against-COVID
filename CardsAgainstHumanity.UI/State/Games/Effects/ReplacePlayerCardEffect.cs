using System.Threading.Tasks;
using CardsAgainstHumanity.Application.Models.Api;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class ReplacePlayerCardEffect : CommonUpdateGameEffect<ReplacePlayerCardAction>
    {
        private readonly IApiClient apiClient;

        public ReplacePlayerCardEffect(IApiClient apiClient) : base(apiClient)
        {
            this.apiClient = apiClient;
        }

        protected override async Task HandleAsync(ReplacePlayerCardAction action, IDispatcher dispatcher)
        {
            await this.apiClient.ReplaceCard(action.InstanceName, new ReplacePlayerCardRequest()
            {
                PlayerId = action.PlayerId,
                CardIndex = action.CardIndex
            });
            await this.TryUpdateGame(action, dispatcher);
        }
    }
}
