using System.Threading.Tasks;
using CardsAgainstHumanity.UI.Clients;
using CardsAgainstHumanity.UI.State.Games.Actions;
using Fluxor;

namespace CardsAgainstHumanity.UI.State.Games.Actions
{
    public class FetchingGameAction
    {
        public readonly bool Fetching;

        public FetchingGameAction(bool fetching) 
        {
            this.Fetching = fetching;
        }
    }
}
