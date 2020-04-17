using System.IO;
using System.Threading.Tasks;
using CardsAgainstHumanity.Application.State;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CardsAgainstHumanity.Api.Extensions
{
    public static class FunctionX
    {
        public static async Task<T> Body<T>(this HttpRequest req)
        {
            var content = await new StreamReader(req.Body).ReadToEndAsync();

            return JsonConvert.DeserializeObject<T>(content);
        }

        public static async Task<T> BodyParam<T>(this HttpRequest req, string property)
        {
            var content = await new StreamReader(req.Body).ReadToEndAsync();
            var response = JObject.Parse(content);
            return response[property].Value<T>();
        }

        public static string RouteParam(this HttpRequest req, string property)
        {
            return req.RouteValues[property].ToString();
        }

        public static async Task<IActionResult> Orchestrate(this IDurableEntityClient context, HttpRequest req, string operationName, IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var name = req.RouteValues["instance"].ToString();
            var entityId = new EntityId(nameof(Game), name);

            await context.SignalEntityAsync(entityId, operationName);
            await TrySignalGroupUpdated(name, signalRMessages);

            return new AcceptedResult();
        }

        public static async Task<IActionResult> Orchestrate<TModel>(this IDurableEntityClient context, HttpRequest req, string operationName, TModel model, IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var name = req.RouteValues["instance"].ToString();
            var entityId = new EntityId(nameof(Game), name);

            await context.SignalEntityAsync(entityId, operationName, model);
            await TrySignalGroupUpdated(name, signalRMessages);

            return new AcceptedResult();
        }

        private static Task TrySignalGroupUpdated(string gameUrl, IAsyncCollector<SignalRMessage> signalRMessages)
        {
            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "gameUpdated",
                    GroupName = gameUrl,
                    Arguments = new[] { gameUrl }
                });
        }
    }
}
