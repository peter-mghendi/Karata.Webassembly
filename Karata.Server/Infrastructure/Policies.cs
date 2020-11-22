using Microsoft.AspNetCore.Authorization;

namespace Karata.Server.Infrastructure
{
    public class Policies
    {
        public const string Admin = nameof(Admin);
        public const string User = nameof(User);

        public static AuthorizationPolicy AdminPolicy() =>
            new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(Admin).Build();

        public static AuthorizationPolicy UserPolicy() =>
            new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(User).Build();
    }
}
