using Karata.Shared;
using Karata.Shared.Client;
using Karata.Web.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Karata.Web
{
    public class Program
    {
        private const string API_URL = "https://karata-server.herokuapp.com/api";
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("#app");

            builder.Services.AddSingleton<ITokenService, TokenService>();

            builder.Services.AddSingleton<IKarataClient, KarataClient>(sp => new(sp.GetRequiredService<ITokenService>()) { BaseUrl = API_URL });

            await builder.Build().RunAsync();
        }
    }
}
