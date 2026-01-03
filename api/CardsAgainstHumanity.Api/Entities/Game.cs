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
        public Round? CurrentRound { get; set; }

        [ActorTableEntityComplexProperty]
        public IList<Round> PreviousRounds { get; set; } = new List<Round>();

        public Dictionary<int, int> Score => PreviousRounds
            .Where(r => r.WonBy > 0)
            .GroupBy(r => r.WonBy)
            .ToDictionary(g => g.Key, g => g.Count());

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

        public Game ReplacePlayedCards(IList<string> newCards)
        {
            if (PreviousRounds == null || !PreviousRounds.Any())
            {
                return this;
            }

            var lastRound = PreviousRounds.Last();
            if (lastRound?.Responses == null)
            {
                return this;
            }

            // Calculate total cards needed
            var totalCardsNeeded = lastRound.Responses
                .Where(r => r.Responses != null)
                .Sum(r => r.Responses.Count);

            // Validate we have enough cards
            if (newCards.Count < totalCardsNeeded)
            {
                // Log or handle insufficient cards - for now just return to avoid partial replacement
                return this;
            }

            // For each player who responded in the last round, replace their played cards
            foreach (var response in lastRound.Responses)
            {
                var player = Players.FirstOrDefault(p => p.Id == response.PlayerId);
                if (player != null && response.Responses != null && response.Responses.Any())
                {
                    // Sort indices in descending order to avoid index shifting issues
                    var sortedIndices = response.Responses.OrderByDescending(i => i).ToList();
                    
                    foreach (var cardIndex in sortedIndices)
                    {
                        if (cardIndex < player.Cards.Count && newCards.Any())
                        {
                            // Remove the played card
                            player.Cards.RemoveAt(cardIndex);
                            // Add a new card at the same position
                            player.Cards.Insert(cardIndex, newCards.First());
                            // Remove the used new card from the list
                            newCards.RemoveAt(0);
                        }
                    }
                }
            }

            IncrementVersion();
            return this;
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
