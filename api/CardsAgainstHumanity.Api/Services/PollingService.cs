namespace CardsAgainstHumanity.Api.Services;

public interface IPollingService
{
    Task<PollingResponse> Poll(string gameName, int currentVersion, CancellationToken cancellationToken);
}

public class PollingService : IPollingService
{
    private readonly IGameStateService _gameStateService;

    public PollingService(IGameStateService gameStateService)
    {
        _gameStateService = gameStateService;
    }

    public async Task<PollingResponse> Poll(string gameName, int currentVersion, CancellationToken cancellationToken)
    {
        var newVersion = await _gameStateService.WaitForUpdate(gameName, currentVersion, cancellationToken);
        
        return new PollingResponse
        {
            Version = newVersion,
            HasUpdate = newVersion > currentVersion
        };
    }
}

public class PollingResponse
{
    public int Version { get; set; }
    public bool HasUpdate { get; set; }
}
