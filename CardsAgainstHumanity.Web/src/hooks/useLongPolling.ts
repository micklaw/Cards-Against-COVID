// Long polling hook for real-time updates
import { useEffect, useRef } from 'react';
import { gameApi } from '../api/gameApi';
import type { Game } from '../types/game';

export function useLongPolling(
  gameUrl: string,
  currentVersion: number,
  onUpdate: (game: Game) => void,
  enabled: boolean
) {
  const pollingRef = useRef<boolean>(false);
  const timeoutRef = useRef<number | undefined>(undefined);
  const versionRef = useRef(currentVersion);

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
        }

        // Continue polling
        timeoutRef.current = window.setTimeout(() => {
          pollingRef.current = false;
          poll();
        }, 100);
      } catch (error) {
        console.error('Polling error:', error);
        
        // Retry with exponential backoff
        const retryDelay = Math.min(5000, 1000 * 2 ** Math.random());
        timeoutRef.current = window.setTimeout(() => {
          pollingRef.current = false;
          poll();
        }, retryDelay);
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
