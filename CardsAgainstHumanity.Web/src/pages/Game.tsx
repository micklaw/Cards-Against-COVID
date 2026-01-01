import React, { useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { fetchGame, selectGame, selectFetching, updateGame } from '../store/gameSlice';
import { useLongPolling } from '../hooks/useLongPolling';
import GameTabs from '../components/GameTabs';

const Game: React.FC = () => {
  const { instance } = useParams<{ instance: string }>();
  const dispatch = useAppDispatch();
  const game = useAppSelector(selectGame);
  const fetching = useAppSelector(selectFetching);

  // Fetch game on mount
  useEffect(() => {
    if (instance && !game) {
      dispatch(fetchGame(instance));
    }
  }, [instance, game, dispatch]);

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
