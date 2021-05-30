using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestingPlatform.Api.Authentication;
using TestingPlatform.Api.Configuration;
using TestingPlatform.Api.Core;
using TestingPlatform.Api.Models;

namespace TestingPlatform.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ModelsDataContext>(builder =>
                builder.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddAuthentication(AuthConstants.TokenAuthentication)
                .AddScheme<AuthenticationSchemeOptions, TokenAuthenticationHandler>(AuthConstants.TokenAuthentication, null);

            services.AddControllers(options =>
            {
                var authRequiredPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(AuthConstants.TokenAuthentication)
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(authRequiredPolicy));
            });

            services
                .AddAutoMapper()
                .AddScoped<AnswersHandler>()
                .AddSwaggerDocument();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) 
                app.UseDeveloperExceptionPage();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}