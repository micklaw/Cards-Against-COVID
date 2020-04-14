using System.Collections.Generic;
using System.Linq;
using CardsAgainstHumanity.UI.State.Games;
using CardsAgainstHumanity.UI.State.Games.Actions;
using CardsAgainstHumanity.UI.State.Games.Models;
using Fluxor;

namespace CardsAgainstHumanity.UI.State
{
    public class Reducers
    {
        [ReducerMethod]
        public static GameState ReduceGameState(GameState state, UpdateCurrentPlayersStateAction action)
        {
            var game = new Game()
            {
                CardCount = action.Game.CardCount,
                Code = action.Game.Code,
                CurrentRound = action.Game.CurrentRound,
                IsOpen = action.Game.IsOpen,
                IsOver = action.Game.IsOver,
                Name = action.Game.Name,
                Players = action.Game.Players,
                PreviousRounds = action.Game.PreviousRounds,
                Url = action.Game.Url
            };

            var voteTable = game.CurrentRound?.Votes
                ?.GroupBy(i => i)
                .OrderByDescending(i => (i?.Count() ?? 0))
                .ToDictionary(i => i.Key, i => (i?.Count() ?? 0)) ?? new Dictionary<int, int>();

            var hasVoted = action.CurrentPlayerId.HasValue && (game.CurrentRound?.Voted?.Contains(action.CurrentPlayerId.Value) ?? false);

            var roundCount = game?.PreviousRounds?.Count ?? 0;

            if (game.CurrentRound != null)
            {
                roundCount++;
            }

            var overallWinner = game?.PreviousRounds?.Select(i => i.WonBy)
                ?.GroupBy(i => i)
                .OrderByDescending(i => (i?.Count() ?? 0))
                .FirstOrDefault()?.Key;

            var responses = game?.CurrentRound?.Responses
                ?.FirstOrDefault(i => action.CurrentPlayerId.HasValue && i.PlayerId == action.CurrentPlayerId)
                ?.Responses ?? new List<int>();

            return new GameState()
            {
                OverallWinner = game?.Players?.FirstOrDefault(i => overallWinner.HasValue && i.Id == overallWinner),
                HasVoted = hasVoted,
                CurrentResponses = responses,
                RoundCount = roundCount,
                CurrentPlayer = game.Players?.FirstOrDefault(i => action.CurrentPlayerId.HasValue && i.Id == action.CurrentPlayerId),
                VoteTable = voteTable,
                Game = game
            };
        }
    }
}
