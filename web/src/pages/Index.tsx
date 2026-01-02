import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch } from '../store/hooks';
import { createGame } from '../store/gameSlice';
import ThemeToggle from '../components/ThemeToggle';

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
    <>
      <div className="absolute top-4 right-4">
        <ThemeToggle />
      </div>
      <div className="aligner">
        <main role="main" className="inner cover">
          <h1 className="cover-heading">Cards Against COVID-19.</h1>
          <div className="flex justify-center">
            <div className="w-full max-w-md">
              <div className="mb-4">
                <p className="text-center mb-4 text-gray-600 dark:text-gray-300">
                  Start a new game or join an existing one?
                </p>
                <input
                  type="text"
                  maxLength={50}
                  className="game-input text-center"
                  placeholder="Enter the name of the game."
                  value={gameName}
                  onChange={(e) => setGameName(e.target.value)}
                  onKeyPress={handleKeyPress}
                />
              </div>
              <div className="text-center">
                <button type="submit" className="btn btn-primary" onClick={handleInitiateGame}>
                  Start
                </button>
              </div>
            </div>
          </div>
        </main>
      </div>
    </>
  );
};

export default Index;
