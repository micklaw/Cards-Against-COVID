using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CardsAgainstHumanity.Application.Services
{
    public interface ICardService
    {
        IList<string> ShuffleResponses(int take = 7);

        string GetPrompt();
    }

    public class CardService : ICardService
    {
        private readonly Random Random = new Random();

        private readonly IList<string> Prompts = new List<string>();

        private readonly IList<string> Responses = new List<string>();

        public CardService()
        {
            PopulateFromWordList(this.Prompts, "blackwords");
            PopulateFromWordList(this.Responses, "whitewords");
        }

        public IList<string> ShuffleResponses(int take = 7)
        {
            var responses = new List<string>();

            for (var i = 0; i < take; i++)
            {
                int index = Random.Next(Responses.Count);
                responses.Add(Responses[index]);
            }

            return responses;
        }

        public string GetPrompt()
        {
            int index = Random.Next(Prompts.Count);
            return Prompts[index];
        }

        private void PopulateFromWordList(IList<string> collection, string wordlist)
        {
            var assembly = typeof(ICardService).Assembly;
            var resourceStream = assembly.GetManifestResourceStream($"CardsAgainstHumanity.Application.Wordlists.{wordlist}.txt");
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    collection.Add(line);
                }
            }
        }
    }
}