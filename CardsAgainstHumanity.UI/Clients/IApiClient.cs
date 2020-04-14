using System.Threading.Tasks;
using CardsAgainstHumanity.Application.Interfaces;
using CardsAgainstHumanity.Application.Models.Api;
using CardsAgainstHumanity.UI.State.Games.Models;
using Refit;

namespace CardsAgainstHumanity.UI.Clients
{
    public interface IApiClient
    {
        [Post("/game/{instance}")]
        Task<Game> GetOrCreate(string instance, [Body]GetOrCreateRequest request);

        [Post("/game/{instance}/open")]
        Task<Game> Open(string instance);

        [Post("/game/{instance}/close")]
        Task<Game> Close(string instance);

        [Post("/game/{instance}/finish")]
        Task<Game> Finish(string instance);

        [Post("/game/{instance}/round/new")]
        Task<Game> NewRound(string instance);

        [Post("/game/{instance}/round/next")]
        Task<Game> NextRound(string instance);

        [Post("/game/{instance}/round/prompt/new")]
        Task<Game> NewPrompt(string instance);

        [Post("/game/{instance}/round/respond")]
        Task<Game> Respond(string instance, [Body]RoundResponseRequest request);

        [Post("/game/{instance}/round/reveal")]
        Task<Game> Reveal(string instance);

        [Post("/game/{instance}/player/add")]
        Task<Game> AddPlayer(string instance, [Body]AddPlayerRequest request);

        [Post("/game/{instance}/player/cards/shuffle")]
        Task<Game> ShuffleCards(string instance, [Body]ShufflePlayerCardsRequest request);

        [Post("/game/{instance}/player/card/replace")]
        Task<Game> ReplaceCard(string instance, [Body]ReplacePlayerCardRequest request);

        [Post("/game/{instance}/round/vote")]
        Task<Game> Vote(string instance, [Body]RoundVoteRequest request);
    }
}
