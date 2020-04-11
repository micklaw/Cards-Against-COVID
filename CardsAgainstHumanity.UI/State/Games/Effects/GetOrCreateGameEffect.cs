using System;
using System.Threading.Tasks;
using CardsAgainstHumanity.Application.Models.Api;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;
using Newtonsoft.Json;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class GetOrCreateGameEffect : Effect<GetOrCreateGameAction>
    {
        private readonly IApiClient apiClient;

        public GetOrCreateGameEffect(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        protected override async Task HandleAsync(GetOrCreateGameAction action, IDispatcher dispatcher)
        {
            var game = await this.apiClient.GetOrCreate(action.InstanceName, new GetOrCreateRequest() { Name = action.GameName });

            if (game != null)
            {
                dispatcher.Dispatch(new UpdateGameStateAction(game));
                dispatcher.Dispatch(new RedirectToGameAction(action.InstanceName));
            }
        }
    }
}
