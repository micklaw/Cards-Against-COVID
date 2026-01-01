using System.Threading.Tasks;
using CardsAgainstHumanity.Application.Models.Api;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class RoundVoteEffect : CommonUpdateGameEffect<RoundVoteAction>
    {
        private readonly IApiClient apiClient;

        public RoundVoteEffect(IApiClient apiClient) : base(apiClient)
        {
            this.apiClient = apiClient;
        }

        public override async Task HandleAsync(RoundVoteAction action, IDispatcher dispatcher)
        {
            await this.apiClient.Vote(action.InstanceName, new RoundVoteRequest()
            {
                PlayerId = action.PlayerId,
                VoteeId = action.VoteeId
            });
            await this.TryUpdateGame(action, dispatcher);
        }
    }
}
