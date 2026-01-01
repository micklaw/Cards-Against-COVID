using System.Threading.Tasks;
using Blazored.LocalStorage;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class UpdateGameStateEffect : Effect<UpdateGameStateAction>
    {
        private readonly ILocalStorageService localStorage;

        public UpdateGameStateEffect(ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
        }

        public override async Task HandleAsync(UpdateGameStateAction action, IDispatcher dispatcher)
        {
            var currentPlayerId = await this.localStorage.GetItemAsync<int>(action.Game.Url);

            dispatcher.Dispatch(new UpdateCurrentPlayersStateAction(action.Game, currentPlayerId));
        }
    }
}
