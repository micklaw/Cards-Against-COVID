using System.Diagnostics;
using System.Net;
using ActorTableEntities;
using CardsAgainstHumanity.Api.Entities;
using CardsAgainstHumanity.Api.Extensions;
using CardsAgainstHumanity.Api.Services;
using CardsAgainstHumanity.Application.Extensions;
using CardsAgainstHumanity.Application.Models.Api;
using CardsAgainstHumanity.Application.Models.Requests;
using CardsAgainstHumanity.Application.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
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
    private static readonly ActivitySource ActivitySource = new("CardsAgainstHumanity.Api");

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
    public async Task<HttpResponseData> ReadStateTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RoutePrefix + "/read")] HttpRequestData req)
    {
        using var activity = ActivitySource.StartActivity("ReadState");
        var name = req.GetRouteValue("instance");

        _logger.LogInformation("Reading game state for: {GameName}", name);

        var game = await _entityClient.Get<Game>("game", name);

        if (game == null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(game);
        return response;
    }

    [Function(nameof(PollTrigger))]
    public async Task<HttpResponseData> PollTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RoutePrefix + "/poll")] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity("Poll");
        var name = req.GetRouteValue("instance");
        var versionStr = req.Query["version"];
        
        if (!int.TryParse(versionStr, out var currentVersion))
        {
            currentVersion = 0;
        }

        _logger.LogInformation("Polling for game: {GameName}, current version: {Version}", name, currentVersion);

        var result = await _pollingService.Poll(name!, currentVersion, cancellationToken);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(result);
        return response;
    }

    [Function(nameof(CreateTrigger))]
    public async Task<HttpResponseData> CreateTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix)] HttpRequestData req)
    {
        using var activity = ActivitySource.StartActivity("CreateGame");
        var name = await req.ReadBodyParamAsync<string>("name");

        _logger.LogInformation("Creating game: {GameName}", name);

        return await Orchestrate(req, name!, game => game.Create(name!));
    }

    [Function(nameof(OpenTrigger))]
    public async Task<HttpResponseData> OpenTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/open")] HttpRequestData req)
    {
        using var activity = ActivitySource.StartActivity("OpenGame");
        var instance = req.GetRouteValue("instance");

        _logger.LogInformation("Opening game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => game.Open());
    }

    [Function(nameof(CloseTrigger))]
    public async Task<HttpResponseData> CloseTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/close")] HttpRequestData req)
    {
        using var activity = ActivitySource.StartActivity("CloseGame");
        var instance = req.GetRouteValue("instance");

        _logger.LogInformation("Closing game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => game.Close());
    }

    [Function(nameof(FinishTrigger))]
    public async Task<HttpResponseData> FinishTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/finish")] HttpRequestData req)
    {
        using var activity = ActivitySource.StartActivity("FinishGame");
        var instance = req.GetRouteValue("instance");

        _logger.LogInformation("Finishing game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => game.Finish());
    }

    [Function(nameof(RevealTrigger))]
    public async Task<HttpResponseData> RevealTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/reveal")] HttpRequestData req)
    {
        using var activity = ActivitySource.StartActivity("RevealRound");
        var instance = req.GetRouteValue("instance");

        _logger.LogInformation("Revealing round for game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => game.RevealRound());
    }

    [Function(nameof(AddPlayerTrigger))]
    public async Task<HttpResponseData> AddPlayerTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/player/add")] HttpRequestData req)
    {
        using var activity = ActivitySource.StartActivity("AddPlayer");
        var model = await req.ReadBodyAsync<AddPlayerModel>();
        model!.Responses = _cardService.ShuffleResponses();

        var instance = req.GetRouteValue("instance");

        _logger.LogInformation("Adding player {PlayerName} to game: {GameName}", model.PlayerName, instance);

        return await Orchestrate(req, instance!, game => game.AddPlayer(model));
    }

    [Function(nameof(NextRoundTrigger))]
    public async Task<HttpResponseData> NextRoundTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/next")] HttpRequestData req)
    {
        using var activity = ActivitySource.StartActivity("NextRound");
        var prompt = _cardService.GetPrompt();
        var instance = req.GetRouteValue("instance");

        _logger.LogInformation("Next round for game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => game.NextRound(prompt));
    }

    [Function(nameof(NewRoundTrigger))]
    public async Task<HttpResponseData> NewRoundTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/new")] HttpRequestData req)
    {
        using var activity = ActivitySource.StartActivity("NewRound");
        var prompt = _cardService.GetPrompt();
        var instance = req.GetRouteValue("instance");

        _logger.LogInformation("New round for game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => game.NewRound(prompt));
    }

    [Function(nameof(VoteTrigger))]
    public async Task<HttpResponseData> VoteTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/vote")] HttpRequestData req)
    {
        using var activity = ActivitySource.StartActivity("Vote");
        var model = await req.ReadBodyAsync<VoteModel>();
        var instance = req.GetRouteValue("instance");

        _logger.LogInformation("Vote in game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => game.Vote(model!));
    }

    [Function(nameof(NewPromptTrigger))]
    public async Task<HttpResponseData> NewPromptTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/prompt/new")] HttpRequestData req)
    {
        using var activity = ActivitySource.StartActivity("NewPrompt");
        var prompt = _cardService.GetPrompt();
        var instance = req.GetRouteValue("instance");

        _logger.LogInformation("New prompt for game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => game.NewPrompt(prompt));
    }

    [Function(nameof(ShufflePlayerCardsTrigger))]
    public async Task<HttpResponseData> ShufflePlayerCardsTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/player/cards/shuffle")] HttpRequestData req)
    {
        using var activity = ActivitySource.StartActivity("ShuffleCards");
        var model = await req.ReadBodyAsync<ShufflePlayerCardsModel>();
        model!.Responses = _cardService.ShuffleResponses();

        var instance = req.GetRouteValue("instance");

        _logger.LogInformation("Shuffling cards for game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => game.ShufflePlayerCards(model));
    }

    [Function(nameof(ReplacePlayerCardTrigger))]
    public async Task<HttpResponseData> ReplacePlayerCardTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/player/card/replace")] HttpRequestData req)
    {
        using var activity = ActivitySource.StartActivity("ReplaceCard");
        var model = await req.ReadBodyAsync<ReplacePlayerCardRequest>();
        model!.Response = _cardService.ShuffleResponses(1).First();

        var instance = req.GetRouteValue("instance");

        _logger.LogInformation("Replacing card for game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => game.ReplacePlayerCard(model));
    }

    [Function(nameof(ResetResponseTrigger))]
    public async Task<HttpResponseData> ResetResponseTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/respond/reset")] HttpRequestData req)
    {
        using var activity = ActivitySource.StartActivity("ResetResponse");
        var model = await req.ReadBodyAsync<ResetResponseRequest>();
        var instance = req.GetRouteValue("instance");

        _logger.LogInformation("Reset response for game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => game.ResetResponse(model!));
    }

    [Function(nameof(RespondTrigger))]
    public async Task<HttpResponseData> RespondTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RoutePrefix + "/round/respond")] HttpRequestData req)
    {
        using var activity = ActivitySource.StartActivity("Respond");
        var model = await req.ReadBodyAsync<RespondModel>();
        var instance = req.GetRouteValue("instance");

        _logger.LogInformation("Respond for game: {GameName}", instance);

        return await Orchestrate(req, instance!, game => game.Respond(model!));
    }

    private async Task<HttpResponseData> Orchestrate(HttpRequestData req, string name, Action<Game> action)
    {
        await using var state = await _entityClient.GetLocked<Game>("game", name.Slugify());

        action?.Invoke(state.Entity);

        await state.Flush();
        
        // Notify state change for long polling
        _gameStateService.NotifyGameUpdate(name, state.Entity.Version);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(state.Entity);
        return response;
    }
}
