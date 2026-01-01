using System.Collections.Generic;
using System.Linq;
using ActorTableEntities;
using CardsAgainstHumanity.Application.Extensions;
using CardsAgainstHumanity.Application.Interfaces;
using CardsAgainstHumanity.Application.Models;
using CardsAgainstHumanity.Application.Models.Api;
using CardsAgainstHumanity.Application.Models.Requests;

namespace CardsAgainstHumanity.Api.Entities
{
    public class Game : ActorTableEntity, IGame
    {
        public Game()
        {

        }

        public Game(string url)
            : base("game", url)
        {

        }

        public string Url { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public int CardCount { get; set; } = 7;

        public int Version { get; set; } = 0;

        [ActorTableEntityComplexProperty]
        public IList<Player> Players { get; set; } = new List<Player>();

        [ActorTableEntityComplexProperty]
        public Round CurrentRound { get; set; }

        [ActorTableEntityComplexProperty]
        public IList<Round> PreviousRounds { get; set; } = new List<Round>();

        public bool IsOpen { get; set; } = true;

        public bool IsOver { get; set; }

        public Game Create(string name)
        {
            if (string.IsNullOrWhiteSpace(Url))
            {
                Url = name.Slugify();
                Name = name;
                Code = RandomX.Get().ToString();
                IncrementVersion();
            }

            return this;
        }

        public Game Open()
        {
            IsOpen = true;
            IncrementVersion();
            return this;
        }

        public Game Close()
        {
            IsOpen = false;
            IncrementVersion();
            return this;
        }

        public Game Finish()
        {
            IsOver = true;
            IncrementVersion();
            return this;
        }

        public Game RevealRound()
        {
            CurrentRound?.Reveal();
            IncrementVersion();
            return this;
        }

        public Game AddPlayer(AddPlayerModel model)
        {
            if (!IsOpen || IsOver)
            {
                return this;
            }

            int count = Players.Count();
            var player = new Player(++count, model.PlayerName)
            {
                Cards = model.Responses
            };

            Players.Add(player);
            IncrementVersion();
            return this;
        }

        public Game NewRound(string prompt)
        {
            CurrentRound = new Round()
            {
                Prompt = prompt
            };
            IncrementVersion();

            return this;
        }

        public Game NextRound(string prompt)
        {
            if (CurrentRound == null || CurrentRound.WonBy <= 0)
            {
                return this;
            }

            if (PreviousRounds == null)
            {
                PreviousRounds = new List<Round>();
            }

            PreviousRounds.Add(CurrentRound);

            return NewRound(prompt);
        }

        public Game Vote(VoteModel model)
        {
            CurrentRound?.Vote(model.PlayerId, model.VoteeId);
            IncrementVersion();
            return this;
        }

        public Game NewPrompt(string prompt)
        {
            CurrentRound?.NewPrompt(prompt);
            IncrementVersion();
            return this;
        }

        public Game ShufflePlayerCards(ShufflePlayerCardsModel model)
        {
            Player player = Players.First(i => i.Id == model.PlayerId);
            player.Shuffle(model.Responses);
            IncrementVersion();
            return this;
        }

        public Game ReplacePlayerCard(ReplacePlayerCardRequest model)
        {
            Player player = Players.First(i => i.Id == model.PlayerId);
            player.Replace(model.CardIndex, model.Response);
            IncrementVersion();
            return this;
        }

        public Game Respond(RespondModel model)
        {
            CurrentRound?.Respond(model.PlayerId, model.Responses);
            IncrementVersion();
            return this;
        }
        
        public Game ResetResponse(ResetResponseRequest model)
        {
            CurrentRound?.ResetResponse(model.PlayerId);
            IncrementVersion();
            return this;
        }

        private void IncrementVersion()
        {
            Version++;
        }
    }
}
