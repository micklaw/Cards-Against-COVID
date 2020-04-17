using System.Threading.Tasks;
using CardsAgainstHumanity.Application.Extensions;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using CardsAgainstHumanity.UI.State.Games.Models;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public abstract class CommonUpdateGameEffect<T> : Effect<T> where T : BaseGameAction
    {
        private readonly IApiClient apiClient;

        protected CommonUpdateGameEffect(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }
        protected async Task<Game> TryUpdateGame(BaseGameAction action, IDispatcher dispatcher)
        {
            if (string.IsNullOrWhiteSpace(action.InstanceName))
            {
                return null;
            }

            dispatcher.Dispatch(new FetchingGameAction(true));

            await Task.Delay(1000);

            var game = await this.apiClient.WithRetry(c => c.Read(action.InstanceName));

            if (game != null)
            {
                dispatcher.Dispatch(new UpdateGameStateAction(game));
            }

            dispatcher.Dispatch(new FetchingGameAction(false));

            return game;
        }
    }
}
