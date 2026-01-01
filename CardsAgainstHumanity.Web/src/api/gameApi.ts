// API client for backend communication
import axios from 'axios';
import type { AxiosInstance } from 'axios';
import type { Game, PollingResponse } from '../types/game';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:7071/api';

class GameApi {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: API_BASE_URL,
      timeout: 35000, // Slightly longer than polling timeout
      headers: {
        'Content-Type': 'application/json',
      },
    });
  }

  // Read game state
  async getGame(gameUrl: string): Promise<Game> {
    const response = await this.client.get<Game>(`/game/${gameUrl}/read`);
    return response.data;
  }

  // Poll for updates
  async poll(gameUrl: string, currentVersion: number): Promise<PollingResponse> {
    const response = await this.client.get<PollingResponse>(
      `/game/${gameUrl}/poll`,
      {
        params: { version: currentVersion },
      }
    );
    return response.data;
  }

  // Create new game
  async createGame(name: string): Promise<Game> {
    const response = await this.client.post<Game>(`/game/${name}`, { name });
    return response.data;
  }

  // Add player
  async addPlayer(gameUrl: string, playerName: string): Promise<Game> {
    const response = await this.client.post<Game>(
      `/game/${gameUrl}/player/add`,
      { playerName }
    );
    return response.data;
  }

  // Start new round
  async newRound(gameUrl: string): Promise<Game> {
    const response = await this.client.post<Game>(`/game/${gameUrl}/round/new`);
    return response.data;
  }

  // Submit response
  async respond(gameUrl: string, playerId: number, responses: string[]): Promise<Game> {
    const response = await this.client.post<Game>(
      `/game/${gameUrl}/round/respond`,
      { playerId, responses }
    );
    return response.data;
  }

  // Vote for winner
  async vote(gameUrl: string, playerId: number, voteeId: number): Promise<Game> {
    const response = await this.client.post<Game>(
      `/game/${gameUrl}/round/vote`,
      { playerId, voteeId }
    );
    return response.data;
  }

  // Reveal round
  async revealRound(gameUrl: string): Promise<Game> {
    const response = await this.client.post<Game>(`/game/${gameUrl}/round/reveal`);
    return response.data;
  }

  // Close game
  async closeGame(gameUrl: string): Promise<Game> {
    const response = await this.client.post<Game>(`/game/${gameUrl}/close`);
    return response.data;
  }

  // Finish game
  async finishGame(gameUrl: string): Promise<Game> {
    const response = await this.client.post<Game>(`/game/${gameUrl}/finish`);
    return response.data;
  }
}

export const gameApi = new GameApi();
