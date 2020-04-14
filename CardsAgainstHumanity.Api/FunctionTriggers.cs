using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CardsAgainstHumanity.Api.Extensions;
using CardsAgainstHumanity.Api.Models;
using CardsAgainstHumanity.Application.Interfaces;
using CardsAgainstHumanity.Application.Models.Api;
using CardsAgainstHumanity.Application.Services;
using CardsAgainstHumanity.Application.State;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Newtonsoft.Json;

namespace CardsAgainstHumanity.Api
{
    public class FunctionTriggers : FunctionBase
    {
        private const string RoutePrefix = "game/{instance}";
        private readonly ICardService cardService;

        public FunctionTriggers(ICardService cardService)
        {
            this.cardService = cardService;
        }

        [FunctionName(nameof(GetOrCreateTrigger))]
        public async Task<IActionResult> GetOrCreateTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix)] HttpRequest req,
            [DurableClient(TaskHub = "%MyTaskHub%")] IDurableOrchestrationClient context,
            [Queue("game-amended")] IAsyncCollector<IGame> gameEvents)
        {
            var model = await req.BodyParam<string>("name");
            return await context.Orchestrate(req, nameof(Game.GetOrCreate), model, gameEvents);
        }

        [FunctionName(nameof(OpenTrigger))]
        public Task<IActionResult> OpenTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/open")] HttpRequest req,
            [DurableClient(TaskHub = "%MyTaskHub%")] IDurableOrchestrationClient context,
            [Queue("game-amended")] IAsyncCollector<IGame> gameEvents)
            => context.Orchestrate(req, nameof(Game.Open), gameEvents);

        [FunctionName(nameof(CloseTrigger))]
        public Task<IActionResult> CloseTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/close")] HttpRequest req,
            [DurableClient(TaskHub = "%MyTaskHub%")] IDurableOrchestrationClient context,
            [Queue("game-amended")] IAsyncCollector<IGame> gameEvents)
            => context.Orchestrate(req, nameof(Game.Close), gameEvents);

        [FunctionName(nameof(FinishTrigger))]
        public Task<IActionResult> FinishTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/finish")] HttpRequest req,
            [DurableClient(TaskHub = "%MyTaskHub%")] IDurableOrchestrationClient context,
            [Queue("game-amended")] IAsyncCollector<IGame> gameEvents)
            => context.Orchestrate(req, nameof(Game.Finish), gameEvents);

        [FunctionName(nameof(RevealTrigger))]
        public Task<IActionResult> RevealTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/reveal")] HttpRequest req,
            [DurableClient(TaskHub = "%MyTaskHub%")] IDurableOrchestrationClient context,
            [Queue("game-amended")] IAsyncCollector<IGame> gameEvents)
            => context.Orchestrate(req, nameof(Game.RevealRound), gameEvents);
        
        [FunctionName(nameof(AddPlayerTrigger))]
        public async Task<IActionResult> AddPlayerTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/player/add")] HttpRequest req,
            [DurableClient(TaskHub = "%MyTaskHub%")] IDurableOrchestrationClient context,
            [Queue("game-amended")] IAsyncCollector<IGame> gameEvents)
        {
            var model = await req.Body<AddPlayerModel>();
            model.Responses = this.cardService.ShuffleResponses();
            return await context.Orchestrate(req, nameof(Game.AddPlayer), model, gameEvents);
        }

        [FunctionName(nameof(NextRoundTrigger))]
        public async Task<IActionResult> NextRoundTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/next")] HttpRequest req,
            [DurableClient(TaskHub = "%MyTaskHub%")] IDurableOrchestrationClient context,
            [Queue("game-amended")] IAsyncCollector<IGame> gameEvents)
            => await context.Orchestrate(req, nameof(Game.NextRound), this.cardService.GetPrompt(), gameEvents);

        [FunctionName(nameof(NewRoundTrigger))]
        public async Task<IActionResult> NewRoundTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/new")] HttpRequest req,
            [DurableClient(TaskHub = "%MyTaskHub%")] IDurableOrchestrationClient context,
            [Queue("game-amended")] IAsyncCollector<IGame> gameEvents)
            => await context.Orchestrate(req, nameof(Game.NewRound), this.cardService.GetPrompt(), gameEvents);

        [FunctionName(nameof(VoteTrigger))]
        public async Task<IActionResult> VoteTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/vote")] HttpRequest req,
            [DurableClient(TaskHub = "%MyTaskHub%")] IDurableOrchestrationClient context,
            [Queue("game-amended")] IAsyncCollector<IGame> gameEvents)
        {
            var model = await req.Body<VoteModel>();
            return await context.Orchestrate(req, nameof(Game.Vote), model, gameEvents);
        }

        [FunctionName(nameof(NewPromptTrigger))]
        public async Task<IActionResult> NewPromptTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/prompt/new")] HttpRequest req,
            [DurableClient(TaskHub = "%MyTaskHub%")] IDurableOrchestrationClient context,
            [Queue("game-amended")] IAsyncCollector<IGame> gameEvents)
            => await context.Orchestrate(req, nameof(Game.NewPrompt), this.cardService.GetPrompt(), gameEvents);

        [FunctionName(nameof(ShufflePlayerCardsTrigger))]
        public async Task<IActionResult> ShufflePlayerCardsTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/player/cards/shuffle")] HttpRequest req,
            [DurableClient(TaskHub = "%MyTaskHub%")] IDurableOrchestrationClient context,
            [Queue("game-amended")] IAsyncCollector<IGame> gameEvents)
        {
            var model = await req.Body<ShufflePlayerCardsModel>();
            model.Responses = this.cardService.ShuffleResponses();
            return await context.Orchestrate(req, nameof(Game.ShufflePlayerCards), model, gameEvents);
        }

        [FunctionName(nameof(ReplacePlayerCardTrigger))]
        public async Task<IActionResult> ReplacePlayerCardTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/player/card/replace")] HttpRequest req,
            [DurableClient(TaskHub = "%MyTaskHub%")] IDurableOrchestrationClient context,
            [Queue("game-amended")] IAsyncCollector<IGame> gameEvents)
        {
            var model = await req.Body<ReplacePlayerCardRequest>();
            model.Response = this.cardService.ShuffleResponses(1).First();
            return await context.Orchestrate(req, nameof(Game.ReplacePlayerCard), model, gameEvents);
        }

        [FunctionName(nameof(ResetResponseTrigger))]
        public async Task<IActionResult> ResetResponseTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/respond/reset")] HttpRequest req,
            [DurableClient(TaskHub = "%MyTaskHub%")] IDurableOrchestrationClient context,
            [Queue("game-amended")] IAsyncCollector<IGame> gameEvents)
        {
            var model = await req.Body<ResetResponseRequest>();
            return await context.Orchestrate(req, nameof(Game.ResetResponse), model, gameEvents);
        }

        [FunctionName(nameof(RespondTrigger))]
        public async Task<IActionResult> RespondTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/respond")] HttpRequest req,
            [DurableClient(TaskHub = "%MyTaskHub%")] IDurableOrchestrationClient context,
            [Queue("game-amended")] IAsyncCollector<IGame> gameEvents)
        {
            var model = await req.Body<RespondModel>();
            return await context.Orchestrate(req, nameof(Game.Respond), model, gameEvents);
        }
    }
}
