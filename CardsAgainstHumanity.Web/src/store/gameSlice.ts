import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
import { gameApi } from '../api/gameApi';
import type { Game, Player } from '../types/game';

interface GameState {
  game: Game | null;
  currentPlayerId: number | null;
  fetching: boolean;
  error: string | null;
}

const initialState: GameState = {
  game: null,
  currentPlayerId: null,
  fetching: false,
  error: null,
};

// Async thunks
export const createGame = createAsyncThunk(
  'game/create',
  async (gameName: string) => {
    return await gameApi.createGame(gameName);
  }
);

export const fetchGame = createAsyncThunk(
  'game/fetch',
  async (gameUrl: string) => {
    return await gameApi.getGame(gameUrl);
  }
);

export const addPlayer = createAsyncThunk(
  'game/addPlayer',
  async ({ gameUrl, playerName }: { gameUrl: string; playerName: string }) => {
    return await gameApi.addPlayer(gameUrl, playerName);
  }
);

export const newRound = createAsyncThunk(
  'game/newRound',
  async (gameUrl: string) => {
    return await gameApi.newRound(gameUrl);
  }
);

export const newPrompt = createAsyncThunk(
  'game/newPrompt',
  async (gameUrl: string) => {
    return await gameApi.newPrompt(gameUrl);
  }
);

export const respond = createAsyncThunk(
  'game/respond',
  async ({ gameUrl, playerId, responses }: { gameUrl: string; playerId: number; responses: number[] }) => {
    return await gameApi.respond(gameUrl, playerId, responses);
  }
);

export const vote = createAsyncThunk(
  'game/vote',
  async ({ gameUrl, playerId, voteeId }: { gameUrl: string; playerId: number; voteeId: number }) => {
    return await gameApi.vote(gameUrl, playerId, voteeId);
  }
);

export const revealRound = createAsyncThunk(
  'game/revealRound',
  async (gameUrl: string) => {
    return await gameApi.revealRound(gameUrl);
  }
);

export const nextRound = createAsyncThunk(
  'game/nextRound',
  async (gameUrl: string) => {
    return await gameApi.nextRound(gameUrl);
  }
);

export const openGame = createAsyncThunk(
  'game/openGame',
  async (gameUrl: string) => {
    return await gameApi.openGame(gameUrl);
  }
);

export const closeGame = createAsyncThunk(
  'game/closeGame',
  async (gameUrl: string) => {
    return await gameApi.closeGame(gameUrl);
  }
);

export const finishGame = createAsyncThunk(
  'game/finishGame',
  async (gameUrl: string) => {
    return await gameApi.finishGame(gameUrl);
  }
);

export const replaceCard = createAsyncThunk(
  'game/replaceCard',
  async ({ gameUrl, playerId, cardIndex }: { gameUrl: string; playerId: number; cardIndex: number }) => {
    return await gameApi.replaceCard(gameUrl, playerId, cardIndex);
  }
);

export const shuffleCards = createAsyncThunk(
  'game/shuffleCards',
  async ({ gameUrl, playerId }: { gameUrl: string; playerId: number }) => {
    return await gameApi.shuffleCards(gameUrl, playerId);
  }
);

const gameSlice = createSlice({
  name: 'game',
  initialState,
  reducers: {
    setCurrentPlayer: (state, action: PayloadAction<number>) => {
      state.currentPlayerId = action.payload;
    },
    clearError: (state) => {
      state.error = null;
    },
    updateGame: (state, action: PayloadAction<Game>) => {
      state.game = action.payload;
    },
  },
  extraReducers: (builder) => {
    // Create game
    builder.addCase(createGame.pending, (state) => {
      state.fetching = true;
      state.error = null;
    });
    builder.addCase(createGame.fulfilled, (state, action) => {
      state.fetching = false;
      state.game = action.payload;
    });
    builder.addCase(createGame.rejected, (state, action) => {
      state.fetching = false;
      state.error = action.error.message || 'Failed to create game';
    });

    // Fetch game
    builder.addCase(fetchGame.pending, (state) => {
      state.fetching = true;
      state.error = null;
    });
    builder.addCase(fetchGame.fulfilled, (state, action) => {
      state.fetching = false;
      state.game = action.payload;
    });
    builder.addCase(fetchGame.rejected, (state, action) => {
      state.fetching = false;
      state.error = action.error.message || 'Failed to fetch game';
    });

    // Add player
    builder.addCase(addPlayer.fulfilled, (state, action) => {
      state.game = action.payload;
      // Set current player to the last added player
      if (action.payload.players.length > 0) {
        const lastPlayer = action.payload.players[action.payload.players.length - 1];
        state.currentPlayerId = lastPlayer.id;
      }
    });

    // All other actions that update game state
    const updateGameActions = [
      newRound,
      newPrompt,
      respond,
      vote,
      revealRound,
      nextRound,
      openGame,
      closeGame,
      finishGame,
      replaceCard,
      shuffleCards,
    ];

    updateGameActions.forEach((action) => {
      builder.addCase(action.fulfilled, (state, actionPayload) => {
        state.game = actionPayload.payload as Game;
      });
    });
  },
});

export const { setCurrentPlayer, clearError, updateGame } = gameSlice.actions;

// Selectors
export const selectGame = (state: { game: GameState }) => state.game.game;
export const selectCurrentPlayerId = (state: { game: GameState }) => state.game.currentPlayerId;
export const selectCurrentPlayer = (state: { game: GameState }): Player | null => {
  const game = state.game.game;
  const playerId = state.game.currentPlayerId;
  if (!game || playerId === null) return null;
  return game.players.find(p => p.id === playerId) || null;
};
export const selectFetching = (state: { game: GameState }) => state.game.fetching;
export const selectError = (state: { game: GameState }) => state.game.error;

export default gameSlice.reducer;
