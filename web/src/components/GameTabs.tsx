import React, { useState } from 'react';
import { useAppSelector } from '../store/hooks';
import { selectGame, selectCurrentPlayerId } from '../store/gameSlice';
import { Tab } from '../types/game';
import GameTab from './GameTab';
import StatsTab from './StatsTab';
import RoundTab from './RoundTab';
import CardsTab from './CardsTab';
import ThemeToggle from './ThemeToggle';
import { Link } from 'react-router';

interface NavTab {
  tab: Tab;
  title: string;
  active: boolean;
}

const GameTabs: React.FC = () => {
  const game = useAppSelector(selectGame);
  const currentPlayerId = useAppSelector(selectCurrentPlayerId);
  
  const partOfCurrentGame = currentPlayerId !== null;
  
  const [tabs, setTabs] = useState<NavTab[]>([
    { tab: Tab.Stats, title: 'Stats', active: true },
    { tab: Tab.Round, title: 'Round', active: false },
    { tab: Tab.Cards, title: 'Cards', active: false }
  ]);

  const handleToggle = (selectedTab: NavTab) => {
    setTabs(tabs.map(tab => ({
      ...tab,
      active: tab.tab === selectedTab.tab
    })));
  };

  if (!game) return null;

  return (
    <>
      <header className="masthead mb-auto">
        <div className="inner">
          <div className="flex items-center justify-between mb-4">
            <h3 className="masthead-brand">{game.name}</h3>
            <ThemeToggle />
          </div>
          <nav className="nav-masthead">
            {tabs.map(tab => {
              // Hide Cards tab if not part of game or game is over
              if (tab.tab === Tab.Cards && (!partOfCurrentGame || game.isOver)) {
                return null;
              }
              // Hide Round tab if game is over
              if (tab.tab === Tab.Round && game.isOver) {
                return null;
              }
              
              return (
                <a
                  key={tab.tab}
                  className={`nav-link ${tab.active ? 'active' : ''}`}
                  onClick={() => handleToggle(tab)}
                >
                  {tab.title}
                </a>
              );
            })}
            <Link className="nav-link" to="/">New</Link>
          </nav>
        </div>
      </header>

      <main role="main" className="inner cover">
        {tabs.map(tab => (
          <GameTab key={tab.tab} className={tab.active ? '' : 'hidden'}>
            {tab.tab === Tab.Stats && <StatsTab />}
            {tab.tab === Tab.Round && <RoundTab />}
            {tab.tab === Tab.Cards && <CardsTab />}
          </GameTab>
        ))}
      </main>
    </>
  );
};

export default GameTabs;
