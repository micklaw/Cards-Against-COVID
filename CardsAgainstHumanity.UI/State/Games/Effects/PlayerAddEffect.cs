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
    public class PlayerAddEffect : CommonUpdateGameEffect<PlayerAddAction>
    {
        private readonly IApiClient apiClient;
        private readonly ILocalStorageService localStorage;

        public PlayerAddEffect(IApiClient apiClient, ILocalStorageService localStorage) : base(apiClient)
        {
            this.apiClient = apiClient;
            this.localStorage = localStorage;
        }

        public override async Task HandleAsync(PlayerAddAction action, IDispatcher dispatcher)
        {
            await this.apiClient.AddPlayer(action.InstanceName, new AddPlayerRequest()
            {
                PlayerName = action.PlayerName
            });
            var game = await this.TryUpdateGame(action, dispatcher);

            var player = game?.Players.FirstOrDefault(i => i.Name == action.PlayerName);

            if (player != null)
            {
                await this.localStorage.SetItemAsync(game.Url, player.Id);
            }

            await this.TryUpdateGame(action, dispatcher);
        }
    }
}
