using System;
using System.Linq;
using System.Threading.Tasks;
using ActorTableEntities;
using CardsAgainstHumanity.Api.Extensions;
using CardsAgainstHumanity.Application.Models.Api;
using CardsAgainstHumanity.Application.Models.Entities;
using CardsAgainstHumanity.Application.Models.Requests;
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

        public FunctionTriggers(ICardService cardService)
        {
            this.cardService = cardService;
        }

        [FunctionName(nameof(ReadStateTrigger))]
        public async Task<IActionResult> ReadStateTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RoutePrefix + "/read")] HttpRequest req,
            [ActorTableEntity] IActorTableEntityClient entityClient)
        {
            var name = req.RouteValues["instance"].ToString();

            var game = await entityClient.Get<Game>("game", name);

            if (game == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(game);
        }

        [FunctionName(nameof(CreateTrigger))]
        public async Task<IActionResult> CreateTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix)] HttpRequest req,
            [ActorTableEntity] IActorTableEntityClient entityClient,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var name = await req.BodyParam<string>("name");

            return await entityClient.Orchestrate(name, signalRMessages, game => { game.Create(name); });
        }

        [FunctionName(nameof(OpenTrigger))]
        public async Task<IActionResult> OpenTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/open")] HttpRequest req,
            [ActorTableEntity] IActorTableEntityClient entityClient,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var instance = req.RouteValues["instance"].ToString();

            return await entityClient.Orchestrate(instance, signalRMessages, game => game.Open());
        }

        [FunctionName(nameof(CloseTrigger))]
        public async Task<IActionResult> CloseTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/close")] HttpRequest req,
            [ActorTableEntity] IActorTableEntityClient entityClient,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var instance = req.RouteValues["instance"].ToString();

            return await entityClient.Orchestrate(instance, signalRMessages, game => game.Close());
        }

        [FunctionName(nameof(FinishTrigger))]
        public async Task<IActionResult> FinishTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/finish")] HttpRequest req,
            [ActorTableEntity] IActorTableEntityClient entityClient,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var instance = req.RouteValues["instance"].ToString();

            return await entityClient.Orchestrate(instance, signalRMessages, game => game.Finish());
        }

        [FunctionName(nameof(RevealTrigger))]
        public async Task<IActionResult> RevealTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/reveal")] HttpRequest req,
            [ActorTableEntity] IActorTableEntityClient entityClient,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var instance = req.RouteValues["instance"].ToString();

            return await entityClient.Orchestrate(instance, signalRMessages, game => game.RevealRound());
        }

        [FunctionName(nameof(AddPlayerTrigger))]
        public async Task<IActionResult> AddPlayerTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/player/add")] HttpRequest req,
            [ActorTableEntity] IActorTableEntityClient entityClient,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var model = await req.Body<AddPlayerModel>();
            model.Responses = this.cardService.ShuffleResponses();

            var instance = req.RouteValues["instance"].ToString();

            return await entityClient.Orchestrate(instance, signalRMessages, game => game.AddPlayer(model));
        }

        [FunctionName(nameof(NextRoundTrigger))]
        public async Task<IActionResult> NextRoundTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/next")] HttpRequest req,
            [ActorTableEntity] IActorTableEntityClient entityClient,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var prompt = this.cardService.GetPrompt();

            var instance = req.RouteValues["instance"].ToString();

            return await entityClient.Orchestrate(instance, signalRMessages, game => game.NextRound(prompt));
        }

        [FunctionName(nameof(NewRoundTrigger))]
        public async Task<IActionResult> NewRoundTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/new")] HttpRequest req,
            [ActorTableEntity] IActorTableEntityClient entityClient,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var prompt = this.cardService.GetPrompt();

            var instance = req.RouteValues["instance"].ToString();

            return await entityClient.Orchestrate(instance, signalRMessages, game => game.NewRound(prompt));
        }

        [FunctionName(nameof(VoteTrigger))]
        public async Task<IActionResult> VoteTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/vote")] HttpRequest req,
            [ActorTableEntity] IActorTableEntityClient entityClient,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var model = await req.Body<VoteModel>();

            var instance = req.RouteValues["instance"].ToString();

            return await entityClient.Orchestrate(instance, signalRMessages, game => game.Vote(model));
        }

        [FunctionName(nameof(NewPromptTrigger))]
        public async Task<IActionResult> NewPromptTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/prompt/new")] HttpRequest req,
            [ActorTableEntity] IActorTableEntityClient entityClient,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var prompt = this.cardService.GetPrompt();

            var instance = req.RouteValues["instance"].ToString();

            return await entityClient.Orchestrate(instance, signalRMessages, game => game.NewPrompt(prompt));
        }

        [FunctionName(nameof(ShufflePlayerCardsTrigger))]
        public async Task<IActionResult> ShufflePlayerCardsTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/player/cards/shuffle")] HttpRequest req,
            [ActorTableEntity] IActorTableEntityClient entityClient,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var model = await req.Body<ShufflePlayerCardsModel>();
            model.Responses = this.cardService.ShuffleResponses();

            var instance = req.RouteValues["instance"].ToString();

            return await entityClient.Orchestrate(instance, signalRMessages, game => game.ShufflePlayerCards(model));
        }

        [FunctionName(nameof(ReplacePlayerCardTrigger))]
        public async Task<IActionResult> ReplacePlayerCardTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/player/card/replace")] HttpRequest req,
            [ActorTableEntity] IActorTableEntityClient entityClient,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var model = await req.Body<ReplacePlayerCardRequest>();
            model.Response = this.cardService.ShuffleResponses(1).First();

            var instance = req.RouteValues["instance"].ToString();

            return await entityClient.Orchestrate(instance, signalRMessages, game => game.ReplacePlayerCard(model));
        }

        [FunctionName(nameof(ResetResponseTrigger))]
        public async Task<IActionResult> ResetResponseTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/respond/reset")] HttpRequest req,
            [ActorTableEntity] IActorTableEntityClient entityClient,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var model = await req.Body<ResetResponseRequest>();

            var instance = req.RouteValues["instance"].ToString();

            return await entityClient.Orchestrate(instance, signalRMessages, game => game.ResetResponse(model));
        }

        [FunctionName(nameof(RespondTrigger))]
        public async Task<IActionResult> RespondTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/respond")] HttpRequest req,
            [ActorTableEntity] IActorTableEntityClient entityClient,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var model = await req.Body<RespondModel>();

            var instance = req.RouteValues["instance"].ToString();

            return await entityClient.Orchestrate(instance, signalRMessages, game => game.Respond(model));
        }
    }
}
