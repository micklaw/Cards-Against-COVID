using System.Threading.Tasks;
using CardsAgainstHumanity.Application.Models.Api;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class PlayerShuffleCardsEffect : Effect<PlayerShuffleCardsAction>
    {
        private readonly IApiClient apiClient;

        public PlayerShuffleCardsEffect(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        protected override async Task HandleAsync(PlayerShuffleCardsAction action, IDispatcher dispatcher)
        {
            var game = await this.apiClient.ShuffleCards(action.InstanceName, new ShufflePlayerCardsRequest()
            {
                PlayerId = action.PlayerId
            });

            if (game != null)
            {
                dispatcher.Dispatch(new UpdateGameStateAction(game));
            }
        }
    }
}
