import React, { useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { fetchGame, selectGame, selectFetching, selectCurrentPlayerId, updateGame, resetGame, setCurrentPlayer } from '../store/gameSlice';
import { useLongPolling } from '../hooks/useLongPolling';
import { getPlayerCookie } from '../utils/cookies';
import GameTabs from '../components/GameTabs';

const Game: React.FC = () => {
  const { instance } = useParams<{ instance: string }>();
  const dispatch = useAppDispatch();
  const game = useAppSelector(selectGame);
  const fetching = useAppSelector(selectFetching);
  const currentPlayerId = useAppSelector(selectCurrentPlayerId);

  // Fetch game on mount or when instance changes
  useEffect(() => {
    if (instance) {
      // Reset and fetch fresh data if instance changes or doesn't match current game
      if (!game || game.url !== instance) {
        dispatch(resetGame());
        dispatch(fetchGame(instance));
      }
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [instance, dispatch]);

  // Restore player ID from cookie after game is loaded
  useEffect(() => {
    if (game && instance && currentPlayerId === null) {
      const savedPlayerId = getPlayerCookie(instance);
      if (savedPlayerId !== null) {
        // Verify the player still exists in the game
        const playerExists = game.players.some(p => p.id === savedPlayerId);
        if (playerExists) {
          dispatch(setCurrentPlayer(savedPlayerId));
        }
      }
    }
  }, [game, instance, currentPlayerId, dispatch]);

  // Setup long polling for updates
  useLongPolling(
    instance || '',
    game?.version || 0,
    (updatedGame) => {
      dispatch(updateGame(updatedGame));
    },
    !!game
  );

  if (!game) {
    return (
      <div className="aligner">
        <main role="main" className="inner cover">
          <span className="blink">
            Loading{fetching && <span>.</span>}
          </span>
        </main>
      </div>
    );
  }

  return <GameTabs />;
};

export default Game;
