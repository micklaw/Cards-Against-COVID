using CardsAgainstHumanity.Application.Extensions;

namespace CardsAgainstHumanity.UI.State.Games.Actions
{
    public class GetOrCreateGameAction
    {
        public string InstanceName { get; }

        public string GameName { get; }

        public GetOrCreateGameAction(string gameName)
        {
            this.InstanceName = gameName.Slugify();
            this.GameName = gameName;
        }
    }
}
