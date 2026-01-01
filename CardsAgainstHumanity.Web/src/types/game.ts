// Type definitions for game models
export interface Player {
  id: number;
  name: string;
  cards: string[];
  score: number;
}

export interface Round {
  prompt: string;
  responses: Record<number, string[]>;
  isRevealed: boolean;
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
}

export interface PollingResponse {
  version: number;
  hasUpdate: boolean;
}

export interface ApiError {
  message: string;
  status: number;
}
