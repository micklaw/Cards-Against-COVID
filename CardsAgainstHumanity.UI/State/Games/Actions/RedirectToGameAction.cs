using CardsAgainstHumanity.Application.Interfaces;

namespace CardsAgainstHumanity.UI.State.Games.Actions
{
    public class RedirectToGameAction
    {
        public string Instance { get; }

        public RedirectToGameAction(string instance)
        {
            this.Instance = instance;
        }
    }
}
