using System.Linq;
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
            [DurableClient] IDurableEntityClient context)
        {
            var name = req.RouteValues["instance"].ToString();
            var response = await context.ReadEntityStateAsync<Game>(new EntityId(nameof(Game), name));

            if (!response.EntityExists)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(response.EntityState);
        }

        [FunctionName(nameof(CreateTrigger))]
        public async Task<IActionResult> CreateTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix)] HttpRequest req,
            [DurableClient] IDurableEntityClient context,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var model = await req.BodyParam<string>("name");
            return await context.Orchestrate(req, nameof(Game.Create), model, signalRMessages);
        }

        [FunctionName(nameof(OpenTrigger))]
        public Task<IActionResult> OpenTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/open")] HttpRequest req,
            [DurableClient] IDurableEntityClient context,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
            => context.Orchestrate(req, nameof(Game.Open), signalRMessages);

        [FunctionName(nameof(CloseTrigger))]
        public Task<IActionResult> CloseTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/close")] HttpRequest req,
            [DurableClient] IDurableEntityClient context,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
            => context.Orchestrate(req, nameof(Game.Close), signalRMessages);

        [FunctionName(nameof(FinishTrigger))]
        public Task<IActionResult> FinishTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/finish")] HttpRequest req,
            [DurableClient] IDurableEntityClient context,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
            => context.Orchestrate(req, nameof(Game.Finish), signalRMessages);

        [FunctionName(nameof(RevealTrigger))]
        public Task<IActionResult> RevealTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/reveal")] HttpRequest req,
            [DurableClient] IDurableEntityClient context,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
            => context.Orchestrate(req, nameof(Game.RevealRound), signalRMessages);
        
        [FunctionName(nameof(AddPlayerTrigger))]
        public async Task<IActionResult> AddPlayerTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/player/add")] HttpRequest req,
            [DurableClient] IDurableEntityClient context,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var model = await req.Body<AddPlayerModel>();
            model.Responses = this.cardService.ShuffleResponses();
            return await context.Orchestrate(req, nameof(Game.AddPlayer), model, signalRMessages);
        }

        [FunctionName(nameof(NextRoundTrigger))]
        public async Task<IActionResult> NextRoundTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/next")] HttpRequest req,
            [DurableClient] IDurableEntityClient context,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
            => await context.Orchestrate(req, nameof(Game.NextRound), this.cardService.GetPrompt(), signalRMessages);

        [FunctionName(nameof(NewRoundTrigger))]
        public async Task<IActionResult> NewRoundTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/new")] HttpRequest req,
            [DurableClient] IDurableEntityClient context,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
            => await context.Orchestrate(req, nameof(Game.NewRound), this.cardService.GetPrompt(), signalRMessages);

        [FunctionName(nameof(VoteTrigger))]
        public async Task<IActionResult> VoteTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/vote")] HttpRequest req,
            [DurableClient] IDurableEntityClient context,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var model = await req.Body<VoteModel>();
            return await context.Orchestrate(req, nameof(Game.Vote), model, signalRMessages);
        }

        [FunctionName(nameof(NewPromptTrigger))]
        public async Task<IActionResult> NewPromptTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/prompt/new")] HttpRequest req,
            [DurableClient] IDurableEntityClient context,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
            => await context.Orchestrate(req, nameof(Game.NewPrompt), this.cardService.GetPrompt(), signalRMessages);

        [FunctionName(nameof(ShufflePlayerCardsTrigger))]
        public async Task<IActionResult> ShufflePlayerCardsTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/player/cards/shuffle")] HttpRequest req,
            [DurableClient] IDurableEntityClient context,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var model = await req.Body<ShufflePlayerCardsModel>();
            model.Responses = this.cardService.ShuffleResponses();
            return await context.Orchestrate(req, nameof(Game.ShufflePlayerCards), model, signalRMessages);
        }

        [FunctionName(nameof(ReplacePlayerCardTrigger))]
        public async Task<IActionResult> ReplacePlayerCardTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/player/card/replace")] HttpRequest req,
            [DurableClient] IDurableEntityClient context,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var model = await req.Body<ReplacePlayerCardRequest>();
            model.Response = this.cardService.ShuffleResponses(1).First();
            return await context.Orchestrate(req, nameof(Game.ReplacePlayerCard), model, signalRMessages);
        }

        [FunctionName(nameof(ResetResponseTrigger))]
        public async Task<IActionResult> ResetResponseTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/respond/reset")] HttpRequest req,
            [DurableClient] IDurableEntityClient context,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var model = await req.Body<ResetResponseRequest>();
            return await context.Orchestrate(req, nameof(Game.ResetResponse), model, signalRMessages);
        }

        [FunctionName(nameof(RespondTrigger))]
        public async Task<IActionResult> RespondTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/respond")] HttpRequest req,
            [DurableClient] IDurableEntityClient context,
            [SignalR(HubName = "cah")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var model = await req.Body<RespondModel>();
            return await context.Orchestrate(req, nameof(Game.Respond), model, signalRMessages);
        }
    }
}
