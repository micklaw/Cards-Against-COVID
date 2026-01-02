import React, { useState } from 'react';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { selectGame, selectCurrentPlayer, respond, replaceCard } from '../store/gameSlice';
import Card, { CardType } from './Card';

const CardsTab: React.FC = () => {
  const dispatch = useAppDispatch();
  const game = useAppSelector(selectGame);
  const currentPlayer = useAppSelector(selectCurrentPlayer);
  const [hoveredCardIndex, setHoveredCardIndex] = useState<number | null>(null);

  if (!game) return null;

  const currentRound = game.currentRound;
  const currentResponses = currentRound?.responses.find(r => r.playerId === currentPlayer?.id)?.responses || [];

  const handlePlay = (index: number) => {
    if (game && currentPlayer) {
      const newResponses = [...currentResponses, index];
      dispatch(respond({ gameUrl: game.url, playerId: currentPlayer.id, responses: newResponses }));
    }
  };

  const handleUnplay = (index: number) => {
    if (game && currentPlayer) {
      const newResponses = currentResponses.filter(i => i !== index);
      dispatch(respond({ gameUrl: game.url, playerId: currentPlayer.id, responses: newResponses }));
    }
  };

  const handleReplace = (index: number) => {
    if (game && currentPlayer) {
      dispatch(replaceCard({ gameUrl: game.url, playerId: currentPlayer.id, cardIndex: index }));
    }
  };

  if (!currentPlayer) {
    return (
      <div className="mt-4">
        <p className="mb-4 text-gray-600 dark:text-gray-300">You nosey bastard you!</p>
        <div className="mb-4">
          <a href="/" className="text-primary hover:underline">
            Away and find some of your own mates
          </a>
        </div>
      </div>
    );
  }

  return (
    <div className="mt-4">
      {currentPlayer.cards && currentPlayer.cards.length > 0 && (
        <div className="player-cards-stack-container">
          <div className="player-cards-stack">
            {currentPlayer.cards.map((card, index) => {
              const isPlayed = currentResponses.includes(index);
              const isHovered = hoveredCardIndex === index;
              const hasPlayedCard = currentResponses.length > 0;
              
              return (
                <div
                  key={index}
                  className={`player-card-item ${
                    isHovered ? 'hovered' : ''
                  } ${
                    isPlayed ? 'played' : ''
                  }`}
                  onMouseEnter={() => setHoveredCardIndex(index)}
                  onMouseLeave={() => setHoveredCardIndex(null)}
                >
                  <div className="player-card-content">
                    <Card type={CardType.Response} text={card} />
                    <div className="player-card-buttons">
                      {isPlayed ? (
                        <button
                          type="button"
                          className="btn btn-sm btn-warning"
                          onClick={() => handleUnplay(index)}
                        >
                          ↩️ Unplay
                        </button>
                      ) : hasPlayedCard ? (
                        <button
                          type="button"
                          className="btn btn-sm btn-secondary"
                          onClick={() => handleReplace(index)}
                        >
                          🔄 Replace
                        </button>
                      ) : (
                        <>
                          <button
                            type="button"
                            className="btn btn-sm btn-success"
                            onClick={() => handlePlay(index)}
                          >
                            ▶️ Play
                          </button>
                          <button
                            type="button"
                            className="btn btn-sm btn-secondary"
                            onClick={() => handleReplace(index)}
                          >
                            🔄 Replace
                          </button>
                        </>
                      )}
                    </div>
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      )}
    </div>
  );
};

export default CardsTab;
