using System;
using System.IO;
using System.Threading.Tasks;
using ActorTableEntities;
using CardsAgainstHumanity.Api.Entities;
using CardsAgainstHumanity.Application.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
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

        public static async Task<IActionResult> Orchestrate(this IActorTableEntityClient entityClient, string name, IAsyncCollector<SignalRMessage> signalRMessages, Action<Game> action = null)
        {
            await using var state = await entityClient.GetLocked<Game>("game", name.Slugify());

            action?.Invoke(state.Entity);

            await state.Flush();
            await signalRMessages.TrySignalGroupUpdated(name);

            return new OkObjectResult(state.Entity);
        }

        public static Task TrySignalGroupUpdated(this IAsyncCollector<SignalRMessage> signalRMessages, string gameUrl)
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
