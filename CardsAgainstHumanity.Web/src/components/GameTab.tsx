import React from 'react';
import { Tab } from '../types/game';

interface GameTabProps {
  tab: Tab;
  className?: string;
  children: React.ReactNode;
}

const GameTab: React.FC<GameTabProps> = ({ className = 'hidden', children }) => {
  return (
    <div className={className} style={{ width: '100%' }}>
      {children}
    </div>
  );
};

export default GameTab;
