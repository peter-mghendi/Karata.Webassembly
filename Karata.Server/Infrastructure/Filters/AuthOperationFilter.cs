using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Karata.Server.Infrastructure.Filters
{
    public class AuthOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true));

            var authAttributes = attributes.OfType<AuthorizeAttribute>();

            if (authAttributes.Any() && !attributes.OfType<AllowAnonymousAttribute>().Any())
            {
                OpenApiSecurityScheme jwtScheme = new()
                {
                    Reference = new()
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new() { [jwtScheme] = Array.Empty<string>() }
                };

                operation.Responses.Add("401", new() { Description = "Unauthorized" });

                if (authAttributes.Any(a => !string.IsNullOrEmpty(a.Policy)))
                {
                    operation.Responses.Add("403", new() { Description = "Forbidden" });
                }
            }
        }
    }
}
