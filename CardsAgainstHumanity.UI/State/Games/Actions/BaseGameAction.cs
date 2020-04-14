using CardsAgainstHumanity.Application.Extensions;

namespace CardsAgainstHumanity.UI.State.Games.Actions
{
    public abstract class BaseGameAction
    {
        public string InstanceName { get; }

        protected BaseGameAction(string gameName)
        {
            this.InstanceName = gameName.Slugify();
        }
    }
}
