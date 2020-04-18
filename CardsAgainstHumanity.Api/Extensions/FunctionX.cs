using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CardsAgainstHumanity.Application.Extensions;
using CardsAgainstHumanity.Application.Interfaces;
using CardsAgainstHumanity.Application.Models;
using CardsAgainstHumanity.Application.Persistance;
using CardsAgainstHumanity.Application.Persistance.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.WindowsAzure.Storage;
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

        public static async Task<IActionResult> Orchestrate(this DistributedLock mutex, HttpRequest req, IPersistanceProvider<Game> persistance, IAsyncCollector<SignalRMessage> signalRMessages, Func<Game, Game> action)
        {
            return await mutex.LockExecute(async () =>
            {
                var name = req.RouteValues["instance"].ToString();
                var response = await persistance.Get(name);

                if (response.Result == null)
                {
                    return new NotFoundResult();
                }

                var game = action(response.Result);
                var saveResponse = await persistance.InsertOrReplace(game);

                if (!saveResponse.StatusCode.IsSuccess())
                {
                    return new StatusCodeResult(saveResponse.StatusCode);
                }

                await signalRMessages.TrySignalGroupUpdated(name);

                return new OkObjectResult(saveResponse.Result);
            });
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

        public static async Task<IActionResult> LockExecute(this DistributedLock mutex, Func<Task<IActionResult>> action)
        {
            try
            {
                await mutex.AcquireAsync();
            }
            catch (StorageException)
            {
                return new ConflictResult();
            }

            var job = action();

            var timer = new System.Timers.Timer(10000);

            timer.Elapsed += async (sender, e) =>
            {
                if (job.IsCompleted)
                {
                    await mutex.ReleaseAsync();
                }
                else
                {
                    await mutex.RenewAsync();
                }
            };

            timer.Start();

            var result = await job.ConfigureAwait(false);

            await mutex.ReleaseAsync();

            timer.Stop();
            timer.Dispose();

            return result;
        }
    }
}
