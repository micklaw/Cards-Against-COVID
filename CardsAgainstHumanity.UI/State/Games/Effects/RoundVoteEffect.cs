using System.Threading.Tasks;
using CardsAgainstHumanity.Application.Models.Api;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class RoundVoteEffect : Effect<RoundVoteAction>
    {
        private readonly IApiClient apiClient;

        public RoundVoteEffect(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        protected override async Task HandleAsync(RoundVoteAction action, IDispatcher dispatcher)
        {
            var game = await this.apiClient.Vote(action.InstanceName, new RoundVoteRequest()
            {
                PlayerId = action.PlayerId,
                VoteeId = action.VoteeId
            });

            if (game != null)
            {
                dispatcher.Dispatch(new UpdateGameStateAction(game));
            }
        }
    }
}
