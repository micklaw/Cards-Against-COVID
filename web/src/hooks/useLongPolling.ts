// Long polling hook for real-time updates
import { useEffect, useRef } from 'react';
import { gameApi } from '../api/gameApi';
import type { Game } from '../types/game';

const MIN_POLL_INTERVAL = 500;      // Start at 500ms
const MAX_POLL_INTERVAL = 8000;     // Max out at 8 seconds (between 5-10s range)
const BACKOFF_MULTIPLIER = 1.5;     // Increase by 50% each time

export function useLongPolling(
  gameUrl: string,
  currentVersion: number,
  onUpdate: (game: Game) => void,
  enabled: boolean
) {
  const pollingRef = useRef<boolean>(false);
  const timeoutRef = useRef<number | undefined>(undefined);
  const versionRef = useRef(currentVersion);
  const pollIntervalRef = useRef(MIN_POLL_INTERVAL);

  // Update version ref when it changes
  useEffect(() => {
    versionRef.current = currentVersion;
  }, [currentVersion]);

  useEffect(() => {
    if (!enabled || !gameUrl) {
      return;
    }

    async function poll() {
      if (pollingRef.current || !gameUrl) return;

      pollingRef.current = true;

      try {
        const result = await gameApi.poll(gameUrl, versionRef.current);
        
        if (result.hasUpdate) {
          // Fetch the updated game state
          const updatedGame = await gameApi.getGame(gameUrl);
          onUpdate(updatedGame);
          
          // Reset backoff on successful update detection
          pollIntervalRef.current = MIN_POLL_INTERVAL;
        } else {
          // No update - increase backoff interval
          pollIntervalRef.current = Math.min(
            pollIntervalRef.current * BACKOFF_MULTIPLIER,
            MAX_POLL_INTERVAL
          );
        }

        // Continue polling with current interval
        timeoutRef.current = window.setTimeout(() => {
          pollingRef.current = false;
          poll();
        }, pollIntervalRef.current);
      } catch (error) {
        console.error('Polling error:', error);
        
        // On error, use max backoff interval
        pollIntervalRef.current = MAX_POLL_INTERVAL;
        timeoutRef.current = window.setTimeout(() => {
          pollingRef.current = false;
          poll();
        }, pollIntervalRef.current);
      }
    }

    poll();

    return () => {
      pollingRef.current = false;
      if (timeoutRef.current) {
        clearTimeout(timeoutRef.current);
      }
    };
  }, [gameUrl, enabled, onUpdate]);
}
