// Long polling hook for real-time updates
import { useEffect, useRef } from 'react';
import { gameApi } from '../api/gameApi';
import type { Game } from '../types/game';

const MIN_POLL_INTERVAL = 1500;     // Start at 3 seconds
const MAX_POLL_INTERVAL = 10000;    // Max out at 15 seconds
const BACKOFF_MULTIPLIER = 1.5;     // Increase by 50% each time
const POST_UPDATE_DELAY = 3000;     // Wait 5 seconds after receiving an update

// Global tracking to prevent duplicate polling loops across React Strict Mode remounts
const activePollers = new Set<string>();

export function useLongPolling(
  gameUrl: string,
  currentVersion: number,
  onUpdate: (game: Game) => void,
  enabled: boolean
) {
  // Use refs for callback and version to avoid stale closures
  const onUpdateRef = useRef(onUpdate);
  const versionRef = useRef(currentVersion);

  // Update refs when props change
  useEffect(() => {
    onUpdateRef.current = onUpdate;
  }, [onUpdate]);

  useEffect(() => {
    versionRef.current = currentVersion;
  }, [currentVersion]);

  useEffect(() => {
    if (!enabled || !gameUrl) {
      return;
    }

    // Prevent duplicate polling loops (React Strict Mode protection)
    if (activePollers.has(gameUrl)) {
      console.log(`[useLongPolling] Skipping duplicate poller for ${gameUrl}`);
      return;
    }

    activePollers.add(gameUrl);
    console.log(`[useLongPolling] Starting poller for ${gameUrl}`);

    let isCancelled = false;
    let timeoutId: number | undefined;
    let pollInterval = MIN_POLL_INTERVAL;

    async function poll() {
      if (isCancelled) return;

      try {
        const result = await gameApi.poll(gameUrl, versionRef.current);

        if (isCancelled) return;

        if (result.hasUpdate) {
          // Fetch the updated game state
          const updatedGame = await gameApi.getGame(gameUrl);
          
          if (isCancelled) return;
          
          onUpdateRef.current(updatedGame);
          
          // After receiving an update, use a longer delay
          pollInterval = POST_UPDATE_DELAY;
        } else {
          // No update - increase backoff interval
          pollInterval = Math.min(pollInterval * BACKOFF_MULTIPLIER, MAX_POLL_INTERVAL);
        }
      } catch (error) {
        console.error('Polling error:', error);
        // On error, use max backoff interval
        pollInterval = MAX_POLL_INTERVAL;
      }

      // Schedule next poll if not cancelled
      if (!isCancelled) {
        timeoutId = window.setTimeout(poll, pollInterval);
      }
    }

    // Start polling
    poll();

    // Cleanup function
    return () => {
      console.log(`[useLongPolling] Stopping poller for ${gameUrl}`);
      isCancelled = true;
      activePollers.delete(gameUrl);
      if (timeoutId !== undefined) {
        clearTimeout(timeoutId);
      }
    };
  }, [gameUrl, enabled]); // Only re-run when gameUrl or enabled changes
}

