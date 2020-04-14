using System.Threading.Tasks;
using CardsAgainstHumanity.Api.Extensions;
using CardsAgainstHumanity.Api.Models;
using CardsAgainstHumanity.Application.Models.Api;
using CardsAgainstHumanity.Application.State;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace CardsAgainstHumanity.Api
{
    public class FunctionOrchestrators : FunctionBase
    {
        [FunctionName(nameof(Game.GetOrCreate))]
        public Task<Game> GetOrCreate([OrchestrationTrigger] IDurableOrchestrationContext context) => context.Operate<string, Game>(nameof(Game.GetOrCreate));

        [FunctionName(nameof(Game.Open))]
        public Task<Game> Open([OrchestrationTrigger] IDurableOrchestrationContext context) => context.Operate<Game>(nameof(Game.Open));

        [FunctionName(nameof(Game.Close))]
        public Task<Game> Close([OrchestrationTrigger] IDurableOrchestrationContext context) => context.Operate<Game>(nameof(Game.Close));

        [FunctionName(nameof(Game.Finish))]
        public Task<Game> Finish([OrchestrationTrigger] IDurableOrchestrationContext context) => context.Operate<Game>(nameof(Game.Finish));

        [FunctionName(nameof(Game.RevealRound))]
        public Task<Game> RevealRound([OrchestrationTrigger] IDurableOrchestrationContext context) => context.Operate<string, Game>(nameof(Game.RevealRound));

        [FunctionName(nameof(Game.AddPlayer))]
        public Task<Game> AddPlayer([OrchestrationTrigger] IDurableOrchestrationContext context) => context.Operate<AddPlayerModel, Game>(nameof(Game.AddPlayer));

        [FunctionName(nameof(Game.NewRound))]
        public Task<Game> NewRound([OrchestrationTrigger] IDurableOrchestrationContext context) => context.Operate<string, Game>(nameof(Game.NewRound));

        [FunctionName(nameof(Game.NextRound))]
        public Task<Game> NextRound([OrchestrationTrigger] IDurableOrchestrationContext context) => context.Operate<string, Game>(nameof(Game.NextRound));

        [FunctionName(nameof(Game.Vote))]
        public Task<Game> Vote([OrchestrationTrigger] IDurableOrchestrationContext context) => context.Operate<VoteModel, Game>(nameof(Game.Vote));

        [FunctionName(nameof(Game.NewPrompt))]
        public Task<Game> NewPrompt([OrchestrationTrigger] IDurableOrchestrationContext context) => context.Operate<string, Game>(nameof(Game.NewPrompt));

        [FunctionName(nameof(Game.ShufflePlayerCards))]
        public Task<Game> ShufflePlayerCards([OrchestrationTrigger] IDurableOrchestrationContext context) => context.Operate<ShufflePlayerCardsModel, Game>(nameof(Game.ShufflePlayerCards));

        [FunctionName(nameof(Game.ReplacePlayerCard))]
        public Task<Game> ReplacePlayerCard([OrchestrationTrigger] IDurableOrchestrationContext context) => context.Operate<ReplacePlayerCardRequest, Game>(nameof(Game.ReplacePlayerCard));

        [FunctionName(nameof(Game.ResetResponse))]
        public Task<Game> ResetResponse([OrchestrationTrigger] IDurableOrchestrationContext context) => context.Operate<ResetResponseRequest, Game>(nameof(Game.ResetResponse));

        [FunctionName(nameof(Game.Respond))]
        public Task<Game> Respond([OrchestrationTrigger] IDurableOrchestrationContext context) => context.Operate<RespondModel, Game>(nameof(Game.Respond));
    }
}
