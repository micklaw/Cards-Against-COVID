import React, { useState, useEffect } from 'react';
import { useAppSelector } from '../store/hooks';
import { selectGame, selectCurrentPlayerId } from '../store/gameSlice';
import { selectMessages } from '../store/chatSlice';
import { Tab } from '../types/game';
import GameTab from './GameTab';
import StatsTab from './StatsTab';
import RoundTab from './RoundTab';
import CardsTab from './CardsTab';
import ChatTab from './ChatTab';
import ThemeToggle from './ThemeToggle';
import ShareButton from './ShareButton';
import { Link } from 'react-router';
import { getChatVisitCookie } from '../utils/cookies';

interface NavTab {
  tab: Tab;
  title: string;
  active: boolean;
}

const GameTabs: React.FC = () => {
  const game = useAppSelector(selectGame);
  const currentPlayerId = useAppSelector(selectCurrentPlayerId);
  const messages = useAppSelector(selectMessages);
  
  const partOfCurrentGame = currentPlayerId !== null;
  
  const [tabs, setTabs] = useState<NavTab[]>([
    { tab: Tab.Stats, title: 'Stats', active: true },
    { tab: Tab.Round, title: 'Round', active: false },
    { tab: Tab.Cards, title: 'Cards', active: false },
    { tab: Tab.Chat, title: 'Chat', active: false }
  ]);

  const [hasUnreadMessages, setHasUnreadMessages] = useState(false);

  // Check for unread messages
  useEffect(() => {
    if (!game || messages.length === 0) {
      setHasUnreadMessages(false);
      return;
    }

    const lastVisit = getChatVisitCookie(game.url);
    if (!lastVisit) {
      setHasUnreadMessages(messages.length > 0);
      return;
    }

    // Check if there are any messages after the last visit
    const hasUnread = messages.some(msg => new Date(msg.timestamp) > lastVisit);
    setHasUnreadMessages(hasUnread);
  }, [messages, game]);

  const handleToggle = (selectedTab: NavTab) => {
    setTabs(tabs.map(tab => ({
      ...tab,
      active: tab.tab === selectedTab.tab
    })));

    // Clear unread badge when visiting chat tab
    if (selectedTab.tab === Tab.Chat) {
      setHasUnreadMessages(false);
    }
  };

  if (!game) return null;

  return (
    <>
      <header className="masthead mb-auto">
        <div className="inner">
          <div className="flex items-center justify-between mb-4">
            <h3 className="masthead-brand">{game.name}</h3>
            <div className="flex items-center gap-2">
              <ShareButton gameUrl={game.url} />
              <Link className="btn btn-primary btn-sm" to="/">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 24 24"
                  strokeWidth={2}
                  stroke="currentColor"
                  className="w-4 h-4 mr-1"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    d="M12 4.5v15m7.5-7.5h-15"
                  />
                </svg>
              </Link>
              <ThemeToggle />
            </div>
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
                  className={`nav-link ${tab.active ? 'active' : ''} relative`}
                  onClick={() => handleToggle(tab)}
                >
                  {tab.title}
                  {tab.tab === Tab.Chat && hasUnreadMessages && !tab.active && (
                    <span className="absolute top-0 right-0 w-2 h-2 bg-red-500 rounded-full"></span>
                  )}
                </a>
              );
            })}
          </nav>
        </div>
      </header>

      <main role="main" className="inner cover">
        {tabs.map(tab => (
          <GameTab key={tab.tab} className={tab.active ? '' : 'hidden'}>
            {tab.tab === Tab.Stats && <StatsTab />}
            {tab.tab === Tab.Round && <RoundTab />}
            {tab.tab === Tab.Cards && <CardsTab />}
            {tab.tab === Tab.Chat && <ChatTab />}
          </GameTab>
        ))}
      </main>
    </>
  );
};

export default GameTabs;
