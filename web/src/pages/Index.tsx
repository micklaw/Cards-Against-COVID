import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch } from '../store/hooks';
import { createGame } from '../store/gameSlice';

const Index: React.FC = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const [gameName, setGameName] = useState('');

  const handleInitiateGame = async () => {
    if (gameName.trim()) {
      try {
        const result = await dispatch(createGame(gameName.trim())).unwrap();
        // Navigate to the game page
        navigate(`/game/${result.url}`);
      } catch (error) {
        console.error('Failed to create game:', error);
      }
    }
  };

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      handleInitiateGame();
    }
  };

  return (
    <div className="aligner">
      <main role="main" className="inner cover">
        <h1 className="cover-heading">Cards Against COVID-19.</h1>
        <div className="row text-center justify-content-center">
          <div className="col-12 col-md-8 col-lg-6">
            <div className="form-group">
              <p>Start a new game or join and existing one?</p>
              <input
                type="text"
                maxLength={50}
                className="form-control text-center game-input"
                placeholder="Enter the name of the game."
                value={gameName}
                onChange={(e) => setGameName(e.target.value)}
                onKeyPress={handleKeyPress}
              />
            </div>
            <button type="submit" className="btn btn-primary" onClick={handleInitiateGame}>
              Start
            </button>
          </div>
        </div>
      </main>
    </div>
  );
};

export default Index;
