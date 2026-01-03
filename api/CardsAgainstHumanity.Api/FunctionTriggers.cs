using ActorTableEntities;
using CardsAgainstHumanity.Api.Entities;
using CardsAgainstHumanity.Api.Extensions;
using CardsAgainstHumanity.Api.Services;
using CardsAgainstHumanity.Application.Extensions;
using CardsAgainstHumanity.Application.Models.Api;
using CardsAgainstHumanity.Application.Models.Requests;
using CardsAgainstHumanity.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CardsAgainstHumanity.Api;

public class FunctionTriggers
{
    public const string RoutePrefix = "game/{instance}";

    private readonly ICardService _cardService;
    private readonly IActorTableEntityClient _entityClient;
    private readonly IGameStateService _gameStateService;
    private readonly IPollingService _pollingService;
    private readonly ILogger<FunctionTriggers> _logger;

    public FunctionTriggers(
        ICardService cardService,
        IActorTableEntityClient entityClient,
        IGameStateService gameStateService,
        IPollingService pollingService,
        ILogger<FunctionTriggers> logger)
    {
        _cardService = cardService;
        _entityClient = entityClient;
        _gameStateService = gameStateService;
        _pollingService = pollingService;
        _logger = logger;
    }

    [Function(nameof(ReadStateTrigger))]
    public async Task<IActionResult> ReadStateTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RoutePrefix + "/read")] HttpRequest req,
        string instance)
    {
        _logger.LogInformation("Reading game state for: {GameName}", instance);

        var game = await _entityClient.Get<Game>("game", instance);

        if (game == null)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(game);
    }

    [Function(nameof(PollTrigger))]
    public async Task<IActionResult> PollTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RoutePrefix + "/poll")] HttpRequest req,
        string instance,
        CancellationToken cancellationToken)
    {
        var versionStr = req.Query["version"];
        
        if (!int.TryParse(versionStr, out var currentVersion))
        {
            currentVersion = 0;
        }

        _logger.LogInformation("Polling for game: {GameName}, current version: {Version}", instance, currentVersion);

        var result = await _pollingService.Poll(instance!, currentVersion, cancellationToken);

        return new OkObjectResult(result);
    }

    [Function(nameof(CreateTrigger))]
    public async Task<IActionResult> CreateTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix)] HttpRequest req)
    {
        var name = await req.ReadBodyParamAsync<string>("name");

        _logger.LogInformation("Creating game: {GameName}", name);

        return await Orchestrate(req, name!, game => game.Create(name!));
    }

    [Function(nameof(OpenTrigger))]
    public async Task<IActionResult> OpenTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/open")] HttpRequest req,
        string instance)
    {
        _logger.LogInformation("Opening game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => game.Open());
    }

    [Function(nameof(CloseTrigger))]
    public async Task<IActionResult> CloseTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/close")] HttpRequest req,
        string instance)
    {
        _logger.LogInformation("Closing game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => game.Close());
    }

    [Function(nameof(FinishTrigger))]
    public async Task<IActionResult> FinishTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/finish")] HttpRequest req,
        string instance)
    {
        _logger.LogInformation("Finishing game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => game.Finish());
    }

    [Function(nameof(RevealTrigger))]
    public async Task<IActionResult> RevealTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/reveal")] HttpRequest req,
        string instance)
    {
        _logger.LogInformation("Revealing round for game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => game.RevealRound());
    }

    [Function(nameof(AddPlayerTrigger))]
    public async Task<IActionResult> AddPlayerTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/player/add")] HttpRequest req,
        string instance)
    {
        var model = await req.ReadBodyAsync<AddPlayerModel>();
        model!.Responses = _cardService.ShuffleResponses();

        _logger.LogInformation("Adding player {PlayerName} to game: {GameName}", model.PlayerName, instance);

        return await Orchestrate(req, instance!, game => game.AddPlayer(model));
    }

    [Function(nameof(NextRoundTrigger))]
    public async Task<IActionResult> NextRoundTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/next")] HttpRequest req,
        string instance)
    {
        var prompt = _cardService.GetPrompt();

        _logger.LogInformation("Next round for game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => {
            // Calculate how many cards need to be replaced
            var cardCount = 0;
            if (game.CurrentRound?.Responses != null)
            {
                foreach (var response in game.CurrentRound.Responses)
                {
                    if (response.Responses != null)
                    {
                        cardCount += response.Responses.Count;
                    }
                }
            }

            // Generate new cards
            var newCards = _cardService.ShuffleResponses(cardCount);
            
            // Move to next round and replace played cards
            game.NextRound(prompt);
            game.ReplacePlayedCards(newCards);
        });
    }

    [Function(nameof(NewRoundTrigger))]
    public async Task<IActionResult> NewRoundTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/new")] HttpRequest req,
        string instance)
    {
        var prompt = _cardService.GetPrompt();

        _logger.LogInformation("New round for game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => game.NewRound(prompt));
    }

    [Function(nameof(VoteTrigger))]
    public async Task<IActionResult> VoteTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/vote")] HttpRequest req,
        string instance)
    {
        var model = await req.ReadBodyAsync<VoteModel>();

        _logger.LogInformation("Vote in game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => game.Vote(model!));
    }

    [Function(nameof(NewPromptTrigger))]
    public async Task<IActionResult> NewPromptTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/prompt/new")] HttpRequest req,
        string instance)
    {
        var prompt = _cardService.GetPrompt();

        _logger.LogInformation("New prompt for game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => game.NewPrompt(prompt));
    }

    [Function(nameof(ShufflePlayerCardsTrigger))]
    public async Task<IActionResult> ShufflePlayerCardsTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/player/cards/shuffle")] HttpRequest req,
        string instance)
    {
        var model = await req.ReadBodyAsync<ShufflePlayerCardsModel>();
        model!.Responses = _cardService.ShuffleResponses();

        _logger.LogInformation("Shuffling cards for game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => game.ShufflePlayerCards(model));
    }

    [Function(nameof(ReplacePlayerCardTrigger))]
    public async Task<IActionResult> ReplacePlayerCardTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/player/card/replace")] HttpRequest req,
        string instance)
    {
        var model = await req.ReadBodyAsync<ReplacePlayerCardRequest>();
        model!.Response = _cardService.ShuffleResponses(1).First();

        _logger.LogInformation("Replacing card for game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => game.ReplacePlayerCard(model));
    }

    [Function(nameof(ResetResponseTrigger))]
    public async Task<IActionResult> ResetResponseTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/respond/reset")] HttpRequest req,
        string instance)
    {
        var model = await req.ReadBodyAsync<ResetResponseRequest>();

        _logger.LogInformation("Reset response for game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => game.ResetResponse(model!));
    }

    [Function(nameof(RespondTrigger))]
    public async Task<IActionResult> RespondTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/respond")] HttpRequest req,
        string instance)
    {
        var model = await req.ReadBodyAsync<RespondModel>();

        _logger.LogInformation("Respond for game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => game.Respond(model!));
    }

    private async Task<IActionResult> Orchestrate(HttpRequest req, string name, Action<Game> action)
    {
        await using var state = await _entityClient.GetLocked<Game>("game", name.Slugify());

        action?.Invoke(state.Entity);

        await state.Flush();

        _gameStateService.NotifyGameUpdate(name, state.Entity.Version);

        return new OkObjectResult(state.Entity);
    }
}
