using Flurl.Http;
using Karata.Shared.Services;
using System.Net.Http;

namespace Karata.Shared.Client
{
    public partial class KarataClient : IKarataClient
    {
        public string BaseUrl { get; init; } = "";

        public KarataClient(IAuthenticationService authenticationService)
        {
            FlurlHttp.Configure(settings =>
            {
                settings.BeforeCall = call =>
                {
                    if (call.Request.Url.Path != "/api/v1/tokens" || call.Request.Verb != HttpMethod.Post)
                        call.Request = call.Request.WithOAuthBearerToken(authenticationService.Token);
                };
            });
        }
    }
}
