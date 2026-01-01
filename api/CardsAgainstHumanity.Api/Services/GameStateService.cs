using System.Collections.Concurrent;

namespace CardsAgainstHumanity.Api.Services;

public interface IGameStateService
{
    void NotifyGameUpdate(string gameName, int version);
    Task<int> WaitForUpdate(string gameName, int currentVersion, CancellationToken cancellationToken);
    int GetCurrentVersion(string gameName);
}

public class GameStateService : IGameStateService
{
    private readonly ConcurrentDictionary<string, GameStateTracker> _gameStates = new();

    public void NotifyGameUpdate(string gameName, int version)
    {
        var tracker = _gameStates.GetOrAdd(gameName, _ => new GameStateTracker());
        tracker.UpdateVersion(version);
    }

    public async Task<int> WaitForUpdate(string gameName, int currentVersion, CancellationToken cancellationToken)
    {
        var tracker = _gameStates.GetOrAdd(gameName, _ => new GameStateTracker());
        return await tracker.WaitForUpdate(currentVersion, cancellationToken);
    }

    public int GetCurrentVersion(string gameName)
    {
        if (_gameStates.TryGetValue(gameName, out var tracker))
        {
            return tracker.CurrentVersion;
        }
        return 0;
    }

    private class GameStateTracker
    {
        private readonly SemaphoreSlim _semaphore = new(0);
        private int _version = 0;
        private readonly object _lock = new();

        public int CurrentVersion
        {
            get
            {
                lock (_lock)
                {
                    return _version;
                }
            }
        }

        public void UpdateVersion(int version)
        {
            lock (_lock)
            {
                _version = version;
            }
            // Release all waiting threads
            try
            {
                _semaphore.Release(int.MaxValue / 2);
            }
            catch (SemaphoreFullException)
            {
                // Ignore if semaphore is already released
            }
        }

        public async Task<int> WaitForUpdate(int currentVersion, CancellationToken cancellationToken)
        {
            // Check if already updated
            lock (_lock)
            {
                if (_version > currentVersion)
                {
                    return _version;
                }
            }

            // Wait for update with timeout
            try
            {
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
                
                await _semaphore.WaitAsync(linkedCts.Token);
                
                lock (_lock)
                {
                    return _version;
                }
            }
            catch (OperationCanceledException)
            {
                // Timeout or cancellation - return current version
                lock (_lock)
                {
                    return _version;
                }
            }
        }
    }
}
