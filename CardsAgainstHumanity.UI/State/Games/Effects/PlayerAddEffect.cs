using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using CardsAgainstHumanity.Application.Models.Api;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class PlayerAddEffect : Effect<PlayerAddAction>
    {
        private readonly IApiClient apiClient;
        private readonly ILocalStorageService localStorage;

        public PlayerAddEffect(IApiClient apiClient, ILocalStorageService localStorage)
        {
            this.apiClient = apiClient;
            this.localStorage = localStorage;
        }

        protected override async Task HandleAsync(PlayerAddAction action, IDispatcher dispatcher)
        {
            var game = await this.apiClient.AddPlayer(action.InstanceName, new AddPlayerRequest()
            {
                PlayerName = action.PlayerName
            });

            var player = game?.Players.FirstOrDefault(i => i.Name == action.PlayerName);

            if (player != null)
            {
                Console.WriteLine(player.Id);

                await this.localStorage.SetItemAsync(game.Url, player.Id);
            }

            if (game != null)
            {
                dispatcher.Dispatch(new UpdateGameStateAction(game));
            }
        }
    }
}
