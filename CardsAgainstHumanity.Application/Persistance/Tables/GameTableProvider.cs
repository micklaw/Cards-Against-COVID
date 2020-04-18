
using CardsAgainstHumanity.Application.Extensions;
using CardsAgainstHumanity.Application.Interfaces;
using CardsAgainstHumanity.Application.Persistance.Models.Entities;

namespace CardsAgainstHumanity.Application.Persistance.Tables
{
    public class GameTableProvider : PersistanceProviderBase<Game, Game>, IPersistanceProvider<Game>
    {
        public const string PartitionKey = "game";

        public GameTableProvider(TableStorageProvider storageProvider)
            : base(storageProvider)
        {
        }

        protected override Game ToEntity(Game model)
        {
            model.CheckNotNull(nameof(model));
            model.Url.CheckNotNull(nameof(model.Url));

            return new Game(ToKey(model.Url))
            {
                Url = ToKey(model.Url),
                ETag = "*", // ML - Always overwrite for now as we are locking with retry
                CardCount = model.CardCount,
                Code = model.Code,
                CurrentRound = model.CurrentRound,
                IsOpen = model.IsOpen,
                IsOver = model.IsOver,
                Name = model.Name,
                PartitionKey = PartitionKey,
                Players = model.Players,
                PreviousRounds = model.PreviousRounds
            };
        }

        protected override Game ToModel(Game model)
        {
            model.CheckNotNull(nameof(model));
            model.RowKey.CheckNotNull(nameof(model.RowKey));

            return new Game(model.RowKey)
            {
                Url = ToKey(model.Url),
                ETag = model.ETag,
                CardCount = model.CardCount,
                Code = model.Code,
                CurrentRound = model.CurrentRound,
                IsOpen = model.IsOpen,
                IsOver = model.IsOver,
                Name = model.Name,
                PartitionKey = PartitionKey,
                Players = model.Players,
                PreviousRounds = model.PreviousRounds
            };
        }
    }
}
