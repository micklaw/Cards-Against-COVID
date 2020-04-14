using System;
using System.Collections.Generic;
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

        protected override async Task HandleAsync(UpdateGameStateAction action, IDispatcher dispatcher)
        {
            Console.Write(action.Game.Url);

            var currentPlayerId = await this.localStorage.GetItemAsync<int>(action.Game.Url);
            var currentPlayerIdNullable = await this.localStorage.GetItemAsync<int?>(action.Game.Url);
            var currentPlayerIdString = await this.localStorage.GetItemAsync<int?>(action.Game.Url);

            Console.Write("int: " + currentPlayerId);
            Console.Write("nullable: " + currentPlayerIdNullable);
            Console.Write("string: " + currentPlayerIdString);

            dispatcher.Dispatch(new UpdateCurrentPlayersStateAction(action.Game, currentPlayerId));
        }
    }
}
