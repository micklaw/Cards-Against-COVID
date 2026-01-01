using System.Collections.Generic;
using System.Linq;

namespace CardsAgainstHumanity.Application.Models
{
    public class Round
    {
        public string Prompt { get; set; }

        public bool HasResponses => Responses != null && Responses.Any();

        public IList<Response> Responses { get; set; } = new List<Response>();

        public IList<int> Voted { get; set; } = new List<int>();

        public IList<int> Votes { get; set; } = new List<int>();

        public bool IsWon => WonBy > 0;

        public int WonBy { get; set; }

        public void Reveal()
        {
            if (Votes != null && Votes.Any())
            {
                WonBy = Votes.GroupBy(i => i)
                    .Select(i => new {PlayerId = i.Key, Count = i.Count()})
                    .OrderByDescending(i => i.Count)?
                    .FirstOrDefault()?.PlayerId ?? 0;
            }
        }

        public void NewPrompt(string prompt)
        {
            Prompt = prompt;
        }

        public void Vote(int voterId, int id)
        {
            if (Responses == null)
            {
                Responses = new List<Response>();
            }

            if (Responses.All(i => i.PlayerId != id))
            {
                return;
            }

            if (Voted.Contains(voterId))
            {
                return;
            }

            Voted.Add(voterId);
            Votes.Add(id);
        }

        public void ResetResponse(int playerId)
        {
            if (Responses == null)
            {
                Responses = new List<Response>();
            }

            var response = Responses?.FirstOrDefault(i => i.PlayerId == playerId);

            if (response != null)
            {
                Responses.Remove(response);
            }
        }

        public void Respond(int responderId, List<int> responses)
        {
            if (Responses == null)
            {
                Responses = new List<Response>();
            }

            var response = Responses?.FirstOrDefault(i => i.PlayerId == responderId);

            if (response != null)
            {
                Responses.Remove(response);
            }

            Responses.Add(new Response()
            {
                PlayerId = responderId,
                Responses = responses
            });
        }
    }
}
