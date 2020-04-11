using CardsAgainstHumanity.Application.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(CardsAgainstHumanity.Api.Startup))]
namespace CardsAgainstHumanity.Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<ICardService, CardService>();
        }
    }
}
