using System;
using System.Threading.Tasks;
using CardsAgainstHumanity.Application.Models.Api;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Effects
{
    public class ConnectToGameGroupEffect : Effect<ConnectToGameGroupAction>
    {
        private readonly IApiClient apiClient;
        private readonly IState<ConnectionState> connectionState;

        public ConnectToGameGroupEffect(IApiClient apiClient, IState<ConnectionState> connectionState)
        {
            this.apiClient = apiClient;
            this.connectionState = connectionState;
        }

        protected override async Task HandleAsync(ConnectToGameGroupAction action, IDispatcher dispatcher)
        {
            if (connectionState.Value.Connected)
            {
                await this.apiClient.Leave(action.InstanceName, new GroupGameRequest()
                {
                    ConnectionId = connectionState.Value.ConnectionId,
                    UserId = connectionState.Value.UserId
                });

                dispatcher.Dispatch(new ConnectToGameGroupCompleteAction(action.InstanceName, action.ConnectionId, connectionState.Value.UserId, false));
            }

            var userId = connectionState.Value.UserId ?? Guid.NewGuid().ToString().ToLower();

            await this.apiClient.Join(action.InstanceName, new GroupGameRequest()
            {
                ConnectionId = action.ConnectionId,
                UserId = userId
            });

            dispatcher.Dispatch(new ConnectToGameGroupCompleteAction(action.InstanceName, action.ConnectionId, userId, true));
        }
    }
}
