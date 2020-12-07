using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Specialized;
using System.Web;

namespace Karata.Web.Helpers
{
    public static class ExtensionMethods
    {
        public static NameValueCollection QueryString(this NavigationManager navigationManager) =>
            HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);

        public static string QueryString(this NavigationManager navigationManager, string key) =>
            navigationManager.QueryString()[key];
    }
}
