// Type definitions for game models
export interface Player {
  id: number;
  name: string;
  cards: string[];
}

export interface Response {
  playerId: number;
  responses: number[]; // Indices of cards in player's hand
}

export interface Round {
  prompt: string;
  responses: Response[];
  voted: number[];
  votes: number[];
  hasResponses: boolean;
  isWon: boolean;
  wonBy: number;
}

export interface Game {
  url: string;
  name: string;
  code: string;
  cardCount: number;
  version: number;
  players: Player[];
  currentRound?: Round;
  previousRounds: Round[];
  isOpen: boolean;
  isOver: boolean;
  isChatEnabled: boolean;
  score: Record<number, number>;
}

export interface PollingResponse {
  version: number;
  hasUpdate: boolean;
}

export interface ApiError {
  message: string;
  status: number;
}

export interface ChatMessage {
  messageId: string;
  gameId: string;
  userId: number;
  content: string;
  quotedMessageId?: string;
  timestamp: string;
}

export const Tab = {
  Stats: 'stats',
  Round: 'round',
  Cards: 'cards',
  Chat: 'chat'
} as const;

export type Tab = typeof Tab[keyof typeof Tab];

