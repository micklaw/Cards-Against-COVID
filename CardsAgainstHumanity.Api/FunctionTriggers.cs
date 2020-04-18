using System;
using System.Linq;
using System.Threading.Tasks;
using CardsAgainstHumanity.Api.Extensions;
using CardsAgainstHumanity.Application.Extensions;
using CardsAgainstHumanity.Application.Interfaces;
using CardsAgainstHumanity.Application.Models.Api;
using CardsAgainstHumanity.Application.Models.Requests;
using CardsAgainstHumanity.Application.Persistance;
using CardsAgainstHumanity.Application.Persistance.Models.Entities;
using CardsAgainstHumanity.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace CardsAgainstHumanity.Api
{
    public class FunctionTriggers : FunctionBase
    {
        public const string RoutePrefix = "game/{instance}";

        private readonly ICardService cardService;
        private readonly IPersistanceProvider<Game> persistence;
        private readonly DistributedLock mutex;

        public FunctionTriggers(ICardService cardService, IPersistanceProvider<Game> persistence, DistributedLock mutex)
        {
            this.cardService = cardService;
            this.persistence = persistence;
            this.mutex = mutex;
        }

        [FunctionName(nameof(ReadStateTrigger))]
        public async Task<IActionResult> ReadStateTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RoutePrefix + "/read")] HttpRequest req)
        {
            var name = req.RouteValues["instance"].ToString();
            var response = await persistence.Get(name);

            if (response?.Result == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(response.Result);
        }

        [FunctionName(nameof(CreateTrigger))]
        public async Task<IActionResult> CreateTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix)] HttpRequest req,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var model = await req.BodyParam<string>("name");
            var instance = model.Slugify();

            return await mutex.LockExecute(async () =>
            {
                var response = await persistence.Get(instance);
                var game = response.Result;

                if (game == null)
                {
                    game = new Game(instance);
                    game = game.Create(model);
                }

                var saveResponse = await persistence.InsertOrReplace(game);

                if (!saveResponse.StatusCode.IsSuccess())
                {
                    return new StatusCodeResult(saveResponse.StatusCode);
                }

                await signalRMessages.TrySignalGroupUpdated(instance);

                return new OkObjectResult(saveResponse.Result);
            });


        }

        [FunctionName(nameof(OpenTrigger))]
        public async Task<IActionResult> OpenTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/open")] HttpRequest req,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            return await mutex.Orchestrate(
                req,
                persistence,
                signalRMessages,
                game => game.Open());
        }

        [FunctionName(nameof(CloseTrigger))]
        public async Task<IActionResult> CloseTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/close")] HttpRequest req,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            return await mutex.Orchestrate(
                req,
                persistence,
                signalRMessages,
                game => game.Close());
        }

        [FunctionName(nameof(FinishTrigger))]
        public async Task<IActionResult> FinishTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/finish")] HttpRequest req,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            return await mutex.Orchestrate(
                req,
                persistence,
                signalRMessages,
                game => game.Finish());
        }

        [FunctionName(nameof(RevealTrigger))]
        public async Task<IActionResult> RevealTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/reveal")] HttpRequest req,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            return await mutex.Orchestrate(
                req,
                persistence,
                signalRMessages,
                game => game.RevealRound());
        }

        [FunctionName(nameof(AddPlayerTrigger))]
        public async Task<IActionResult> AddPlayerTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/player/add")] HttpRequest req,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var model = await req.Body<AddPlayerModel>();
            model.Responses = this.cardService.ShuffleResponses();

            return await mutex.Orchestrate(
                req,
                persistence,
                signalRMessages,
                game => game.AddPlayer(model));
        }

        [FunctionName(nameof(NextRoundTrigger))]
        public async Task<IActionResult> NextRoundTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/next")] HttpRequest req,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var prompt = this.cardService.GetPrompt();

            return await mutex.Orchestrate(
                req,
                persistence,
                signalRMessages,
                game => game.NextRound(prompt));
        }

        [FunctionName(nameof(NewRoundTrigger))]
        public async Task<IActionResult> NewRoundTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/new")] HttpRequest req,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var prompt = this.cardService.GetPrompt();

            return await mutex.Orchestrate(
                req,
                persistence,
                signalRMessages,
                game => game.NewRound(prompt));
        }

        [FunctionName(nameof(VoteTrigger))]
        public async Task<IActionResult> VoteTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/vote")] HttpRequest req,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var model = await req.Body<VoteModel>();

            return await mutex.Orchestrate(
                req,
                persistence,
                signalRMessages,
                game => game.Vote(model));
        }

        [FunctionName(nameof(NewPromptTrigger))]
        public async Task<IActionResult> NewPromptTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/prompt/new")] HttpRequest req,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var prompt = this.cardService.GetPrompt();

            return await mutex.Orchestrate(
                req,
                persistence,
                signalRMessages,
                game => game.NewPrompt(prompt));
        }

        [FunctionName(nameof(ShufflePlayerCardsTrigger))]
        public async Task<IActionResult> ShufflePlayerCardsTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/player/cards/shuffle")] HttpRequest req,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var model = await req.Body<ShufflePlayerCardsModel>();
            model.Responses = this.cardService.ShuffleResponses();

            return await mutex.Orchestrate(
                req,
                persistence,
                signalRMessages,
                game => game.ShufflePlayerCards(model));
        }

        [FunctionName(nameof(ReplacePlayerCardTrigger))]
        public async Task<IActionResult> ReplacePlayerCardTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/player/card/replace")] HttpRequest req,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var model = await req.Body<ReplacePlayerCardRequest>();
            model.Response = this.cardService.ShuffleResponses(1).First();

            return await mutex.Orchestrate(
                req,
                persistence,
                signalRMessages,
                game => game.ReplacePlayerCard(model));
        }

        [FunctionName(nameof(ResetResponseTrigger))]
        public async Task<IActionResult> ResetResponseTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/respond/reset")] HttpRequest req,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var model = await req.Body<ResetResponseRequest>();

            return await mutex.Orchestrate(
                req,
                persistence,
                signalRMessages,
                game => game.ResetResponse(model));
        }

        [FunctionName(nameof(RespondTrigger))]
        public async Task<IActionResult> RespondTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/respond")] HttpRequest req,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var model = await req.Body<RespondModel>();

            return await mutex.Orchestrate(
                req,
                persistence,
                signalRMessages,
                game => game.Respond(model));
        }
    }
}
