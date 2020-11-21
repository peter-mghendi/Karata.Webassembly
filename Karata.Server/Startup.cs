using Karata.Server.Data;
using Karata.Server.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Npgsql;
using System;

namespace Karata.Server
{
    public class Startup
    {
        private const string CorsPolicy = nameof(CorsPolicy);
        private const string Development = nameof(Development);

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicy, builder => builder
                    .WithOrigins("https://localhost:4201")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddDbContext<KarataContext>(options =>
            {
                string connectionString;
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Development;

                if (env == Development)
                {
                    connectionString = this.Configuration.GetConnectionString(nameof(KarataContext));
                }
                else
                {
                    Uri databaseUri = new Uri(Environment.GetEnvironmentVariable("DATABASE_URL"));
                    var userInfo = databaseUri.UserInfo.Split(':');

                    connectionString = new NpgsqlConnectionStringBuilder
                    {
                        Host = databaseUri.Host,
                        Port = databaseUri.Port,
                        Username = userInfo[0],
                        Password = userInfo[1],
                        Database = databaseUri.LocalPath.TrimStart('/'),
                        SslMode = SslMode.Prefer,
                        TrustServerCertificate = true
                    }.ToString();
                }

                options.UseNpgsql(connectionString);
            });

            services.AddSignalR();
            services.AddControllers();

            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo() { Title = "Karata.Server", Version = "v1" }));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Karata.Server v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(CorsPolicy);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/hubs/chat");
            });
        }
    }
}
