import React, { useState } from 'react';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import {
  selectGame,
  selectCurrentPlayerId,
  newRound,
  newPrompt,
  revealRound,
  nextRound,
  vote
} from '../store/gameSlice';
import Card, { CardType } from './Card';

const RoundTab: React.FC = () => {
  const dispatch = useAppDispatch();
  const game = useAppSelector(selectGame);
  const currentPlayerId = useAppSelector(selectCurrentPlayerId);
  const [viewingResponses, setViewingResponses] = useState<string[]>([]);

  if (!game) return null;

  const partOfCurrentGame = currentPlayerId !== null;
  const currentRound = game.currentRound;
  const hasVoted = currentRound?.voted.includes(currentPlayerId || -1) || false;

  const getPlayerName = (playerId: number, useIsWon = false): string => {
    if (!game.players || playerId < 0) return 'Unknown';
    
    if (!useIsWon) {
      return game.players.find(p => p.id === playerId)?.name || 'Unknown';
    }
    
    if (currentRound?.isWon) {
      return game.players.find(p => p.id === playerId)?.name || 'Unknown';
    }
    
    return '******';
  };

  const getCurrentRoundPlayerVotes = (playerId: number): number => {
    if (!currentRound) return 0;
    return currentRound.votes.filter(v => v === playerId).length;
  };

  const handleViewResponses = (playerId: number, responseIndices: number[]) => {
    const player = game.players.find(p => p.id === playerId);
    if (!player?.cards) {
      setViewingResponses([]);
      return;
    }

    const responses: string[] = [];
    responseIndices.forEach(index => {
      if (index < player.cards.length) {
        responses.push(player.cards[index]);
      }
    });
    setViewingResponses(responses);
  };

  const handleCloseResponses = () => {
    setViewingResponses([]);
  };

  const handleNewRound = () => {
    if (game) {
      dispatch(newRound(game.url));
    }
  };

  const handleNewPrompt = () => {
    if (game) {
      dispatch(newPrompt(game.url));
    }
  };

  const handleReveal = () => {
    if (game) {
      dispatch(revealRound(game.url));
    }
  };

  const handleNext = () => {
    if (game) {
      dispatch(nextRound(game.url));
    }
  };

  const handleVote = (voteeId: number) => {
    if (game && currentPlayerId !== null) {
      dispatch(vote({ gameUrl: game.url, playerId: currentPlayerId, voteeId }));
    }
  };

  if (!currentRound) {
    return (
      <div className="mt-2">
        <p>No fun has begun</p>
        {partOfCurrentGame && (
          <form>
            <div className="form-group">
              <button type="button" className="btn btn-primary" onClick={handleNewRound}>
                New Round
              </button>
            </div>
          </form>
        )}
      </div>
    );
  }

  return (
    <div className="mt-2">
      <div className="mt-2">
        <Card type={CardType.Prompt} text={currentRound.prompt} />
      </div>

      {viewingResponses.length > 0 && (
        <>
          <ul className="list-unstyled justify-content-center mt-1">
            {viewingResponses.map((response, index) => (
              <li key={index} className="response-list">
                <Card type={CardType.Response} text={response} />
                <div className="invisible">.</div>
              </li>
            ))}
          </ul>
          <p>
            <button type="button" className="btn btn-primary mb-3" onClick={handleCloseResponses}>
              Hide
            </button>
          </p>
        </>
      )}

      {partOfCurrentGame && (
        <>
          <p><i>Change the prompt, or restart round.</i></p>
          <form>
            <div className="form-group">
              <button type="button" className="btn btn-primary" onClick={handleNewPrompt}>
                Change card
              </button>
              {' '}
              <button type="button" className="btn btn-primary" onClick={handleNewRound}>
                Restart round
              </button>
            </div>
          </form>
        </>
      )}

      <h5 className="font-weight-bold">Votes</h5>

      {currentRound.hasResponses ? (
        <>
          <table className="table table-sm table-dark">
            <thead>
              <tr>
                <th>Name</th>
                <th className="text-right">Responses</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {currentRound.responses.map(response => (
                <tr key={response.playerId}>
                  <td>
                    {getPlayerName(response.playerId, true)}
                    {currentRound.isWon && currentRound.wonBy === response.playerId && (
                      <span> ⭐</span>
                    )}
                  </td>
                  <td className="text-right">{getCurrentRoundPlayerVotes(response.playerId)}</td>
                  <td>
                    <div className="float-right">
                      <button
                        type="button"
                        className="btn btn-sm btn-primary"
                        onClick={() => handleViewResponses(response.playerId, response.responses)}
                      >
                        View
                      </button>
                      {' '}
                      {partOfCurrentGame && !hasVoted && (
                        <button
                          type="button"
                          className="btn btn-sm btn-primary"
                          onClick={() => handleVote(response.playerId)}
                        >
                          Vote
                        </button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          {partOfCurrentGame && (
            <form>
              <div className="form-group">
                {!currentRound.isWon && currentRound.hasResponses && (
                  <button type="button" className="btn btn-primary" onClick={handleReveal}>
                    Reveal winner
                  </button>
                )}
                {currentRound.isWon && (
                  <>
                    {' '}
                    <button type="button" className="btn btn-primary" onClick={handleNext}>
                      Next round
                    </button>
                  </>
                )}
              </div>
            </form>
          )}
        </>
      ) : (
        <p>No responses yet, hurry up</p>
      )}
    </div>
  );
};

export default RoundTab;
