using IdentityModel.Client;
using Klika.Identity.Database.DbContexts;
using Klika.Identity.Model.Constants.Assemblies;
using Klika.Identity.Model.Constants.TokenProviders;
using Klika.Identity.Model.Entities;
using Klika.Identity.Service.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace Klika.Identity.Api.Extensions
{
    public static class IdentityServerConfiguration
    {
        public static void AddIdentityServerConfiguration(this IServiceCollection services, IConfiguration _config)
        {
            string identityConnectionString = _config.GetConnectionString("IdentityDbContext");

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+!#$%&'*+-/=?^_`{|}~.";
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
                options.Tokens.EmailConfirmationTokenProvider = CustomTokenProviders.EmailDataProtectorTokenProvider;
                options.Tokens.PasswordResetTokenProvider = CustomTokenProviders.PasswordDataProtectorTokenProvider;
            })
             .AddEntityFrameworkStores<IdentityDbContext>()
             .AddDefaultTokenProviders()
             .AddTokenProvider<EmailConfirmationTokenProvider<ApplicationUser>>(CustomTokenProviders.EmailDataProtectorTokenProvider)
             .AddTokenProvider<PasswordResetTokenProvider<ApplicationUser>>(CustomTokenProviders.PasswordDataProtectorTokenProvider);
            

            services.AddIdentityServer()
                    .AddDeveloperSigningCredential()
                    .AddAspNetIdentity<ApplicationUser>()
                    //Configuration Store: clients and resources
                    .AddConfigurationStore(options =>
                    {
                        options.ConfigureDbContext = db =>
                        db.UseSqlServer(identityConnectionString,
                            sql => sql.MigrationsAssembly(InternalAssemblies.Database));
                    })
                    //Operational Store: tokens, codes etc.
                    .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = db =>
                        db.UseSqlServer(identityConnectionString,
                            sql => sql.MigrationsAssembly(InternalAssemblies.Database));
                    })
                    .AddProfileService<IdentityProfileService>(); // custom claims 

            //Cache Discovery document HttpClient
            services.AddSingleton<IDiscoveryCache>(r =>
            {
                var factory = r.GetRequiredService<IHttpClientFactory>();
                return new DiscoveryCache(_config["AuthApiUrl"], () => factory.CreateClient());
            });
        }
    }
}
