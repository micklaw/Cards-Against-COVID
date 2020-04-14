using System.Threading.Tasks;
using CardsAgainstHumanity.Application.Models.Api;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class RoundRespondEffect : Effect<RoundRespondAction>
    {
        private readonly IApiClient apiClient;

        public RoundRespondEffect(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        protected override async Task HandleAsync(RoundRespondAction action, IDispatcher dispatcher)
        {
            var game = await this.apiClient.Respond(action.InstanceName, new RoundResponseRequest()
            {
                PlayerId = action.PlayerId,
                Responses = action.Responses
            });

            if (game != null)
            {
                dispatcher.Dispatch(new UpdateGameStateAction(game));
            }
        }
    }
}
