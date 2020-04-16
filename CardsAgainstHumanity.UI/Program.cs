using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Blazored.LocalStorage;
using CardsAgainstHumanity.UI.Clients;
using Fluxor;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace CardsAgainstHumanity.UI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            var settings = new RefitSettings();

            builder.Services.AddRefitClient<IApiClient>(settings)
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://funcardshumanity.azurewebsites.net/api"));

            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddFluxor(options =>
            {
                options.ScanAssemblies(typeof(Program).Assembly);

#if DEBUG
                options.UseRouting();
                options.UseReduxDevTools();
#endif
            });

            await builder.Build().RunAsync();
        }
    }
}
