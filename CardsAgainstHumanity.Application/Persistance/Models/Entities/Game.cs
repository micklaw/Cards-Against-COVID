using System.Collections.Generic;
using System.Linq;
using CardsAgainstHumanity.Application.Extensions;
using CardsAgainstHumanity.Application.Interfaces;
using CardsAgainstHumanity.Application.Models;
using CardsAgainstHumanity.Application.Models.Api;
using CardsAgainstHumanity.Application.Models.Requests;
using CardsAgainstHumanity.Application.Persistance.Attributes;

namespace CardsAgainstHumanity.Application.Persistance.Models.Entities
{
    public class Game : ComplexTableEntity, IGame
    {
        public Game()
        {

        }

        public Game(string url)
            : base("game", url)
        {

        }

        public string Url { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public int CardCount { get; set; } = 7;

        [ComplexProperty]
        public IList<Player> Players { get; set; } = new List<Player>();

        [ComplexProperty]
        public Round CurrentRound { get; set; }

        [ComplexProperty]
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
            }

            return this;
        }

        public Game Open()
        {
            IsOpen = true;
            return this;
        }

        public Game Close()
        {
            IsOpen = false;
            return this;
        }

        public Game Finish()
        {
            IsOver = true;
            return this;
        }

        public Game RevealRound()
        {
            CurrentRound?.Reveal();
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
            return this;
        }

        public Game NewRound(string prompt)
        {
            CurrentRound = new Round()
            {
                Prompt = prompt
            };

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
            return this;
        }

        public Game NewPrompt(string prompt)
        {
            CurrentRound?.NewPrompt(prompt);
            return this;
        }

        public Game ShufflePlayerCards(ShufflePlayerCardsModel model)
        {
            Player player = Players.First(i => i.Id == model.PlayerId);
            player.Shuffle(model.Responses);
            return this;
        }

        public Game ReplacePlayerCard(ReplacePlayerCardRequest model)
        {
            Player player = Players.First(i => i.Id == model.PlayerId);
            player.Replace(model.CardIndex, model.Response);
            return this;
        }

        public Game Respond(RespondModel model)
        {
            CurrentRound?.Respond(model.PlayerId, model.Responses);
            return this;
        }
        public Game ResetResponse(ResetResponseRequest model)
        {
            CurrentRound?.ResetResponse(model.PlayerId);
            return this;
        }
    }
}
