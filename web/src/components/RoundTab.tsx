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
  const [hoveredCardIndex, setHoveredCardIndex] = useState<number | null>(null);
  const [selectedCardIndex, setSelectedCardIndex] = useState<number | null>(null);

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

  const getResponseCards = (playerId: number, responseIndices: number[]): string[] => {
    const player = game?.players.find(p => p.id === playerId);
    if (!player?.cards) return [];
    
    return responseIndices
      .map(index => index < player.cards.length ? player.cards[index] : null)
      .filter((card): card is string => card !== null);
  };

  const handleCardClick = (index: number, playerId: number) => {
    if (!partOfCurrentGame || hasVoted || !currentRound) return;
    setSelectedCardIndex(index);
    handleVote(playerId);
  };

  if (!currentRound) {
    return (
      <div className="mt-4">
        <p className="mb-4 text-gray-600 dark:text-gray-300">No fun has begun</p>
        {partOfCurrentGame && (
          <div className="mb-4">
            <button type="button" className="btn btn-primary" onClick={handleNewRound}>
              New Round
            </button>
          </div>
        )}
      </div>
    );
  }

  return (
    <div className="mt-4">
      <div className="mt-4">
        <Card type={CardType.Prompt} text={currentRound.prompt} />
      </div>

      {partOfCurrentGame && (
        <>
          <p className="text-gray-500 dark:text-gray-400 italic mb-4">
            Change the prompt, or restart round.
          </p>
          <div className="flex gap-2 mb-6">
            <button type="button" className="btn btn-primary" onClick={handleNewPrompt}>
              Change card
            </button>
            <button type="button" className="btn btn-primary" onClick={handleNewRound}>
              Restart round
            </button>
          </div>
        </>
      )}

      <h5 className="font-bold text-lg mb-4">Responses</h5>

      {currentRound.hasResponses ? (
        <>
          <div className="card-stack-container">
            <div className="card-stack">
              {currentRound.responses.map((response, index) => {
                const responseCards = getResponseCards(response.playerId, response.responses);
                const isWinner = currentRound.isWon && currentRound.wonBy === response.playerId;
                const isHovered = hoveredCardIndex === index;
                const isSelected = selectedCardIndex === index;
                const shouldShowFront = isWinner && currentRound.isWon;
                const voteCount = getCurrentRoundPlayerVotes(response.playerId);
                
                // Calculate z-index: winner on top, selected next, hovered above others, rest in order
                let zIndex = index;
                if (isHovered && !shouldShowFront) zIndex = 100;
                if (isSelected && !shouldShowFront) zIndex = 101;
                if (shouldShowFront) zIndex = 200;
                
                return (
                  <div
                    key={response.playerId}
                    className={`card-stack-item ${
                      isHovered ? 'hovered' : ''
                    } ${
                      shouldShowFront ? 'winner' : ''
                    } ${
                      isSelected ? 'selected' : ''
                    }`}
                    style={{
                      '--card-index': index,
                      '--z-index': zIndex
                    } as React.CSSProperties}
                    onMouseEnter={() => !shouldShowFront && setHoveredCardIndex(index)}
                    onMouseLeave={() => setHoveredCardIndex(null)}
                    onClick={() => handleCardClick(index, response.playerId)}
                  >
                    <div className="card-stack-content">
                      {responseCards.map((cardText, cardIndex) => (
                        <div key={cardIndex} className="stacked-card-wrapper">
                          <Card type={CardType.Response} text={cardText} />
                        </div>
                      ))}
                    </div>
                    
                    <div className="card-info">
                      <div className="player-name">
                        {getPlayerName(response.playerId, true)}
                        {isWinner && <span className="winner-star"> ⭐</span>}
                      </div>
                      {voteCount > 0 && (
                        <div className="vote-count">
                          {voteCount} {voteCount === 1 ? 'vote' : 'votes'}
                        </div>
                      )}
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
          
          {partOfCurrentGame && !hasVoted && !currentRound.isWon && (
            <p className="voting-instructions">
              <i>Hover over cards to read them, click to vote</i>
            </p>
          )}

          {partOfCurrentGame && (
            <div className="flex gap-2 mt-4">
              {!currentRound.isWon && currentRound.hasResponses && (
                <button type="button" className="btn btn-primary" onClick={handleReveal}>
                  Reveal winner
                </button>
              )}
              {currentRound.isWon && (
                <button type="button" className="btn btn-primary" onClick={handleNext}>
                  Next round
                </button>
              )}
            </div>
          )}
        </>
      ) : (
        <p className="text-gray-500 dark:text-gray-400">No responses yet, hurry up</p>
      )}
    </div>
  );
};

export default RoundTab;
