using System.Threading.Tasks;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace CardsAgainstHumanity.UI.State.Games.Actions
{
    public class RedirectToGameEffect : Effect<RedirectToGameAction>
    {
        private readonly NavigationManager navigationManager;

        public RedirectToGameEffect(NavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
        }

        public override Task HandleAsync(RedirectToGameAction action, IDispatcher dispatcher)
        {
            if (!string.IsNullOrWhiteSpace(action.Instance))
            {
                navigationManager.NavigateTo("/game/" + action.Instance);
            }

            return Task.CompletedTask;
        }
    }
}
