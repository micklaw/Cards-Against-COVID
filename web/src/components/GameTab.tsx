import React from 'react';

interface GameTabProps {
  className?: string;
  children: React.ReactNode;
}

const GameTab: React.FC<GameTabProps> = ({ className = 'hidden', children }) => {
  return (
    <div className={`w-full ${className}`}>
      {children}
    </div>
  );
};

export default GameTab;
