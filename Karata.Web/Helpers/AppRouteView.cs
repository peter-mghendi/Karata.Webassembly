using Karata.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Net;

namespace Karata.Web.Helpers
{
    public class AppRouteView : RouteView
    {
        [Inject]
        public NavigationManager Navigator { get; set; }

        [Inject]
        public IAuthenticationService Authenticator { get; set; }

        private const string Admin = nameof(Admin); 

        protected override void Render(RenderTreeBuilder builder)
        {
            var authorizeAttribute = Attribute.GetCustomAttribute(RouteData.PageType, typeof(AuthorizeAttribute)) as AuthorizeAttribute;

            if (authorizeAttribute is not null && !Authenticator.IsAuthenticated)
            {
                var returnUrl = WebUtility.UrlEncode(new Uri(Navigator.Uri).PathAndQuery);
                Navigator.NavigateTo($"login?returnUrl={returnUrl}");
                return;
            }
            else if (authorizeAttribute?.Policy == Admin && Authenticator.Role != Admin)
            {
                Navigator.NavigateTo("");
                return;
            }
            else base.Render(builder);
        }
    }
}
