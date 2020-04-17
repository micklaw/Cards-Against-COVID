﻿using System.Threading.Tasks;
using CardsAgainstHumanity.Application.Interfaces;
using CardsAgainstHumanity.Application.Models.Api;
using CardsAgainstHumanity.UI.State.Games.Models;
using Refit;

namespace CardsAgainstHumanity.UI.Clients
{
    public interface IApiClient
    {
        [Get("/game/{instance}/read")]
        Task<Game> Read(string instance);

        [Post("/game/{instance}")]
        Task GetOrCreate(string instance, [Body]CreateRequest request);

        [Post("/game/{instance}/open")]
        Task Open(string instance);

        [Post("/game/{instance}/close")]
        Task Close(string instance);

        [Post("/game/{instance}/finish")]
        Task Finish(string instance);

        [Post("/game/{instance}/round/new")]
        Task NewRound(string instance);

        [Post("/game/{instance}/round/next")]
        Task NextRound(string instance);

        [Post("/game/{instance}/round/prompt/new")]
        Task NewPrompt(string instance);

        [Post("/game/{instance}/round/respond")]
        Task Respond(string instance, [Body]RoundResponseRequest request);

        [Post("/game/{instance}/round/reveal")]
        Task Reveal(string instance);

        [Post("/game/{instance}/player/add")]
        Task AddPlayer(string instance, [Body]AddPlayerRequest request);

        [Post("/game/{instance}/player/cards/shuffle")]
        Task ShuffleCards(string instance, [Body]ShufflePlayerCardsRequest request);

        [Post("/game/{instance}/player/card/replace")]
        Task ReplaceCard(string instance, [Body]ReplacePlayerCardRequest request);

        [Post("/game/{instance}/round/vote")]
        Task Vote(string instance, [Body]RoundVoteRequest request);

        [Put("/game/{instance}/join")]
        Task Join(string instance, [Body]GroupGameRequest request);

        [Put("/game/{instance}/leave")]
        Task Leave(string instance, [Body]GroupGameRequest request);
    }
}
