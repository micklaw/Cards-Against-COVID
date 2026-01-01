import React from 'react';

interface GameTabProps {
  className?: string;
  children: React.ReactNode;
}

const GameTab: React.FC<GameTabProps> = ({ className = 'd-none', children }) => {
  return (
    <div className={className} style={{ width: '100%' }}>
      {children}
    </div>
  );
};

export default GameTab;
