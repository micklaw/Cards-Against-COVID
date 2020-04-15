using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CardsAgainstHumanity.Application.Extensions;
using CardsAgainstHumanity.Application.Interfaces;
using CardsAgainstHumanity.Application.State;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
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

        public static async Task<IActionResult> Orchestrate(this IDurableOrchestrationClient context, HttpRequest req, string orchestration, IAsyncCollector<IGame> gameEvents)
        {
            var name = req.RouteValues["instance"].ToString();
            var slug = name.Slugify();
            var instanceId = await context.StartNewAsync(orchestration, slug);
            var result = await context.WaitForCompletionOrCreateCheckStatusResponseAsync(req, instanceId);
            return await TrySignalGroupUpdated(result, gameEvents);
        }

        public static async Task<IActionResult> Orchestrate<TModel>(this IDurableOrchestrationClient context, HttpRequest req, string orchestration, TModel model, IAsyncCollector<IGame> gameEvents)
        {
            var name = req.RouteValues["instance"].ToString();
            var slug = name.Slugify();
            var instanceId = await context.StartNewAsync(orchestration, slug, model);
            var result = await context.WaitForCompletionOrCreateCheckStatusResponseAsync(req, instanceId);
            return await TrySignalGroupUpdated(result, gameEvents);
        }

        public static async Task<TResponse> Operate<TModel, TResponse>(this IDurableOrchestrationContext context, string operation)
        {
            var model = context.GetInput<TModel>();
            var entityId = new EntityId(nameof(Game), context.InstanceId);
            var response = await context.CallEntityAsync<TResponse>(entityId, operation, model);
            return response;
        }

        public static async Task<TResponse> SignalOperate<TModel, TResponse>(this IDurableOrchestrationContext context, string operation)
        {
            var model = context.GetInput<TModel>();
            var entityId = new EntityId(nameof(Game), context.InstanceId);
            var response = await context.CallEntityAsync<TResponse>(entityId, operation, model);
            return response;
        }

        public static async Task<TResponse> Operate<TResponse>(this IDurableOrchestrationContext context, string operation)
        {
            var entityId = new EntityId(nameof(Game), context.InstanceId);
            var response = await context.CallEntityAsync<TResponse>(entityId, operation);
            return response;
        }

        private static async Task<IActionResult> TrySignalGroupUpdated(IActionResult result, IAsyncCollector<IGame> gameEvents)
        {
            if (result is ObjectResult objectResult)
            {
                var response = (HttpResponseMessage)objectResult.Value;
                var game = await response.Content.ReadAsAsync<Game>();
                await gameEvents.AddAsync(game);
            }

            return result;
        }
    }
}
