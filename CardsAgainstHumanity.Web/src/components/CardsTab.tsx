import React, { useState } from 'react';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { selectGame, selectCurrentPlayer, respond, replaceCard, shuffleCards } from '../store/gameSlice';
import Card, { CardType } from './Card';

const CardsTab: React.FC = () => {
  const dispatch = useAppDispatch();
  const game = useAppSelector(selectGame);
  const currentPlayer = useAppSelector(selectCurrentPlayer);
  const [selectedResponses, setSelectedResponses] = useState<number[]>([]);

  if (!game) return null;

  const currentRound = game.currentRound;
  const currentResponses = currentRound?.responses.find(r => r.playerId === currentPlayer?.id)?.responses || [];

  const handleToggle = (index: number) => {
    if (selectedResponses.includes(index)) {
      setSelectedResponses(selectedResponses.filter(i => i !== index));
    } else {
      setSelectedResponses([...selectedResponses, index]);
    }
  };

  const handleReplace = (index: number) => {
    if (game && currentPlayer) {
      dispatch(replaceCard({ gameUrl: game.url, playerId: currentPlayer.id, cardIndex: index }));
    }
  };

  const handleReplaceAll = () => {
    if (game && currentPlayer) {
      dispatch(shuffleCards({ gameUrl: game.url, playerId: currentPlayer.id }));
    }
  };

  const handleResponse = () => {
    if (game && currentPlayer) {
      dispatch(respond({ gameUrl: game.url, playerId: currentPlayer.id, responses: selectedResponses }));
      setSelectedResponses([]);
    }
  };

  const handleResetResponse = () => {
    if (game && currentPlayer) {
      dispatch(respond({ gameUrl: game.url, playerId: currentPlayer.id, responses: [] }));
    }
  };

  if (!currentPlayer) {
    return (
      <div className="mt-2">
        <p>You nosey bastard you!</p>
        <form>
          <div className="form-group">
            <a href="/">Away and find some of your own mates</a>
          </div>
        </form>
      </div>
    );
  }

  return (
    <div className="mt-2">
      {currentPlayer.cards && currentPlayer.cards.length > 0 && (
        <ul className="list-unstyled justify-content-center">
          {currentPlayer.cards.map((card, index) => (
            <li key={index} className="response-list">
              <Card type={CardType.Response} text={card} />
              <form>
                <div className="form-group">
                  {!currentResponses.includes(index) && !selectedResponses.includes(index) ? (
                    <>
                      <button
                        type="button"
                        className="btn btn-sm btn-primary"
                        onClick={() => handleToggle(index)}
                      >
                        Select
                      </button>
                      {' '}
                      <button
                        type="button"
                        className="btn btn-sm btn-primary"
                        onClick={() => handleReplace(index)}
                      >
                        Replace
                      </button>
                    </>
                  ) : selectedResponses.includes(index) ? (
                    <button
                      type="button"
                      className="btn btn-sm btn-warning"
                      onClick={() => handleToggle(index)}
                    >
                      Undo
                    </button>
                  ) : (
                    <button type="button" className="btn btn-sm btn-info" disabled>
                      Played
                    </button>
                  )}
                </div>
              </form>
            </li>
          ))}
        </ul>
      )}

      {currentResponses.length === 0 && selectedResponses.length === 0 && (
        <>
          <p><i>Replace all cards?</i></p>
          <form>
            <div className="form-group">
              <button type="button" className="btn btn-primary" onClick={handleReplaceAll}>
                Replace all
              </button>
            </div>
          </form>
        </>
      )}

      {currentResponses.length > 0 && selectedResponses.length === 0 && (
        <form>
          <div className="form-group">
            <button type="button" className="btn btn-primary" onClick={handleResetResponse}>
              Reset response
            </button>
          </div>
        </form>
      )}

      {selectedResponses.length > 0 && (
        <form>
          <div className="form-group">
            <button type="button" className="btn btn-primary" onClick={handleResponse}>
              Respond
            </button>
          </div>
        </form>
      )}
    </div>
  );
};

export default CardsTab;
