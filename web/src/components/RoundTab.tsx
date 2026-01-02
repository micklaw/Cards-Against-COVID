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

interface ControlButtonsProps {
  currentRound: {
    isWon: boolean;
    hasResponses: boolean;
  };
  onNewPrompt: () => void;
  onNewRound: () => void;
  onReveal: () => void;
  onNext: () => void;
}

const ControlButtons: React.FC<ControlButtonsProps> = ({
  currentRound,
  onNewPrompt,
  onNewRound,
  onReveal,
  onNext
}) => (
  <div className="flex justify-center gap-2 flex-wrap">
    {!currentRound.isWon && (
      <>
        <button type="button" className="btn btn-primary" onClick={onNewPrompt}>
          🔄 Change
        </button>
        <button type="button" className="btn btn-primary" onClick={onNewRound}>
          🔁 Restart
        </button>
        {currentRound.hasResponses && (
          <button type="button" className="btn btn-success" onClick={onReveal}>
            🏆 Winner
          </button>
        )}
      </>
    )}
    {currentRound.isWon && (
      <button type="button" className="btn btn-primary" onClick={onNext}>
        ⏭️ Again?
      </button>
    )}
  </div>
);

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
      <div>
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
      {/* Cards grid - prompt first, then responses */}
      <div className="response-cards-container mt-6">
        <div className="response-cards-grid">
          {/* Prompt card */}
          <div className="response-card-item prompt-card">
            <div className="response-card-content">
              <Card type={CardType.Prompt} text={currentRound.prompt} />
            </div>
          </div>

          {/* Response cards */}
          {currentRound.hasResponses && currentRound.responses.map((response, index) => {
            const responseCards = getResponseCards(response.playerId, response.responses);
            const isWinner = currentRound.isWon && currentRound.wonBy === response.playerId;
            const isHovered = hoveredCardIndex === index;
            const isSelected = selectedCardIndex === index;
            const voteCount = getCurrentRoundPlayerVotes(response.playerId);
            
            return (
              <div
                key={response.playerId}
                className={`response-card-item ${
                  isHovered ? 'hovered' : ''
                } ${
                  isWinner ? 'winner' : ''
                } ${
                  isSelected ? 'selected' : ''
                }`}
                onMouseEnter={() => !isWinner && setHoveredCardIndex(index)}
                onMouseLeave={() => setHoveredCardIndex(null)}
                onClick={() => handleCardClick(index, response.playerId)}
              >
                <div className="response-card-content">
                  {responseCards.map((cardText, cardIndex) => (
                    <div key={cardIndex} className="mb-2 last:mb-0">
                      <Card type={CardType.Response} text={cardText} />
                    </div>
                  ))}
                </div>
                
                <div className="response-card-info">
                  {currentRound.isWon && (
                    <div className="player-name">
                      {getPlayerName(response.playerId)}
                      {isWinner && <span className="winner-star"> ⭐</span>}
                    </div>
                  )}
                  {(currentRound.isWon || isHovered) && voteCount > 0 && (
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

      {/* Controls panel - bottom */}
      {partOfCurrentGame && (
        <div>
          <ControlButtons
            currentRound={currentRound}
            onNewPrompt={handleNewPrompt}
            onNewRound={handleNewRound}
            onReveal={handleReveal}
            onNext={handleNext}
          />
        </div>
      )}
    </div>
  );
};

export default RoundTab;
