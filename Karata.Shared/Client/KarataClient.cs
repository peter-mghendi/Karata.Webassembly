using Flurl.Http;
using System.Net.Http;

namespace Karata.Shared.Client
{
    public partial class KarataClient : IKarataClient
    {
        public string BaseUrl { get; init; } = "";

        public KarataClient(ITokenService tokenService)
        {
            FlurlHttp.Configure(settings =>
            {
                settings.BeforeCall = call =>
                {
                    if (call.Request.Url.Path != "/api/v1/tokens" || call.Request.Verb != HttpMethod.Post)
                        call.Request = call.Request.WithOAuthBearerToken(tokenService.AuthToken);
                };
            });
        }
    }
}
