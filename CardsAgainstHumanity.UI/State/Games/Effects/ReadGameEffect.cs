using System.Threading.Tasks;
using CardsAgainstHumanity.Application.Extensions;
using CardsAgainstHumanity.Application.Models.Api;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class ReadGameEffect : CommonUpdateGameEffect<ReadGameAction>
    {
        private readonly IApiClient apiClient;

        public ReadGameEffect(IApiClient apiClient) : base(apiClient)
        {
            this.apiClient = apiClient;
        }

        public override async Task HandleAsync(ReadGameAction action, IDispatcher dispatcher)
        {
            await TryUpdateGame(action, dispatcher);
        }
    }
}
