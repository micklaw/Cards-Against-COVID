using System.Threading.Tasks;
using CardsAgainstHumanity.Application.State;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace CardsAgainstHumanity.Api
{
    public class FunctionSignalr
    {
        [FunctionName("SignalRNegotiate")]
        public SignalRConnectionInfo GetSignalRInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "negotiate")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "cah")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }

        [FunctionName("GameUpdated")]
        public static async Task GameUpdated(
            [QueueTrigger("game-amended")] Game game,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "gameUpdated",
                    GroupName = game.Url,
                    Arguments = new [] { game }
                });
        }
    }
}
