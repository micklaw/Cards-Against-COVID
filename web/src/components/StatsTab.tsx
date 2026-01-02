import React, { useState } from 'react';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { selectGame, selectCurrentPlayerId, addPlayer, openGame, closeGame, finishGame } from '../store/gameSlice';
import { isGameCreator } from '../utils/cookies';

const StatsTab: React.FC = () => {
  const dispatch = useAppDispatch();
  const game = useAppSelector(selectGame);
  const currentPlayerId = useAppSelector(selectCurrentPlayerId);
  const [playerName, setPlayerName] = useState('');

  if (!game) return null;

  const currentPlayer = game.players.find(p => p.id === currentPlayerId);
  const partOfCurrentGame = currentPlayerId !== null && currentPlayer !== undefined;
  const roundCount = game.previousRounds.length + (game.currentRound ? 1 : 0);
  const isCreator = isGameCreator(game.url);
  
  const overallWinner = game.score && Object.keys(game.score).length > 0
    ? game.players.find(p => p.id === parseInt(
        Object.entries(game.score).sort((a, b) => b[1] - a[1])[0][0]
      ))
    : null;

  const handleAddPlayer = () => {
    if (playerName.trim() && game) {
      dispatch(addPlayer({ gameUrl: game.url, playerName: playerName.trim() }));
      setPlayerName('');
    }
  };

  const handleOpen = () => {
    if (game) {
      dispatch(openGame(game.url));
    }
  };

  const handleClose = () => {
    if (game) {
      dispatch(closeGame(game.url));
    }
  };

  const handleFinish = () => {
    if (game) {
      dispatch(finishGame(game.url));
    }
  };

  return (
    <div className="mt-4">
      <h5 className="font-bold text-lg mb-2">Rounds</h5>
      <p className="mb-4">{roundCount}</p>

      <h5 className="font-bold text-lg mb-2">Scores</h5>
      {game.players && game.players.length > 0 ? (
        <table className="game-table mb-4 rounded-lg overflow-hidden">
          <thead>
            <tr>
              <th className="text-left">Name</th>
              <th className="text-right">Score</th>
            </tr>
          </thead>
          <tbody>
            {game.players.map(player => {
              const score = game.score?.[player.id] || 0;
              return (
                <tr key={player.id}>
                  <td>{player.name}</td>
                  <td className="text-right">{score}</td>
                </tr>
              );
            })}
          </tbody>
        </table>
      ) : (
        <p className="mb-4 text-gray-500 dark:text-gray-400">No players have joined... yet</p>
      )}

      {!game.isOver ? (
        <>
          <h5 className="font-bold text-lg mb-2">Actions</h5>
          <div className="mt-4">
            {game.isOpen && !partOfCurrentGame && (
              <div className="mb-4">
                <div className="flex justify-center">
                  <div className="w-full max-w-md">
                    <div className="input-group">
                      <input
                        type="text"
                        className="game-input text-center"
                        value={playerName}
                        onChange={(e) => setPlayerName(e.target.value)}
                        placeholder="Your name... is shit"
                        aria-label=""
                      />
                      <div className="input-group-append">
                        <button
                          className="btn btn-primary"
                          type="button"
                          onClick={handleAddPlayer}
                        >
                          🎮 Join
                        </button>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            )}
            {game.isOpen && partOfCurrentGame && (
              <div className="alert alert-primary" role="alert">
                Currently playing as <strong>{currentPlayer?.name}</strong>.
              </div>
            )}
            {isCreator && (
              <div className="flex gap-2 mt-4">
                {!game.isOpen ? (
                  <button type="button" className="btn btn-secondary" onClick={handleOpen} title="Open the game">
                    🔒 Closed
                  </button>
                ) : (
                  <button type="button" className="btn btn-success" onClick={handleClose} title="Close the game">
                    🔓 Open
                  </button>
                )}
                <button type="button" className="btn btn-primary" onClick={handleFinish}>
                  🏁 End
                </button>
              </div>
            )}
          </div>
        </>
      ) : (
        <div className="alert alert-primary mt-6" role="alert">
          {overallWinner ? (
            <>Game over!, glory belongs too <strong>{overallWinner.name}</strong>.</>
          ) : (
            <>Game over!, glory belongs too no-one, because you all suck.</>
          )}
        </div>
      )}
    </div>
  );
};

export default StatsTab;
