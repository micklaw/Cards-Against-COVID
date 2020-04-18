using System.Threading.Tasks;
using CardsAgainstHumanity.Api.Extensions;
using CardsAgainstHumanity.Application.Models.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Newtonsoft.Json;

namespace CardsAgainstHumanity.Api
{
    public class FunctionSignalr
    {
        [FunctionName(nameof(SignalRNegotiate))]
        public SignalRConnectionInfo SignalRNegotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "negotiate")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "cah")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }

        [FunctionName(nameof(JoinGroup))]
        public static async Task JoinGroup(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = FunctionTriggers.RoutePrefix + "/join")] HttpRequest req,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRGroupAction> signalRMessages)
        {
            var model = await req.Body<GroupGameRequest>();
            var group = req.RouteParam("instance");

            await signalRMessages.AddAsync(
                new SignalRGroupAction()
                {
                    ConnectionId = model.ConnectionId,
                    UserId = model.UserId,
                    Action = GroupAction.Add,
                    GroupName = group
                });
        }

        [FunctionName(nameof(LeaveGroup))]
        public static async Task LeaveGroup(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = FunctionTriggers.RoutePrefix + "/leave")] HttpRequest req,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRGroupAction> signalRMessages)
        {
            var model = await req.Body<GroupGameRequest>();
            var group = req.RouteParam("instance");

            await signalRMessages.AddAsync(
                new SignalRGroupAction()
                {
                    ConnectionId = model.ConnectionId,
                    UserId = model.UserId,
                    Action = GroupAction.Remove,
                    GroupName = group
                });
        }
    }
}
