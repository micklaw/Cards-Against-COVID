using System.Threading.Tasks;
using CardsAgainstHumanity.Application.Extensions;
using CardsAgainstHumanity.Application.Models.Api;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class CreateGameEffect : Effect<CreateGameAction>
    {
        private readonly IApiClient apiClient;

        public CreateGameEffect(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        protected override async Task HandleAsync(CreateGameAction action, IDispatcher dispatcher)
        {
            var game = await this.apiClient.WithRetry(c => c.Read(action.InstanceName));

            if (game == null)
            {
                await this.apiClient.GetOrCreate(action.InstanceName, new CreateRequest() { Name = action.GameName });
                await Task.Delay(1000);

                game = await this.apiClient.WithRetry(c => c.Read(action.InstanceName));
            }

            if (game != null)
            {
                dispatcher.Dispatch(new UpdateGameStateAction(game));
                dispatcher.Dispatch(new RedirectToGameAction(action.InstanceName));
            }
        }
    }
}
