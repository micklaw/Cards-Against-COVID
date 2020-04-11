using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
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
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:7071/api"));

            builder.Services.AddFluxor(options =>
            {
                options.ScanAssemblies(typeof(Program).Assembly);
                options.UseRouting();
                options.UseReduxDevTools();
            });

            await builder.Build().RunAsync();
        }
    }
}
