using AutoMapper;
using IdentityServer4.Validation;
using Klika.Identity.Api.AutoMapper;
using Klika.Identity.Api.Extensions;
using Klika.Identity.Database.DbContexts;
using Klika.Identity.Model.Configuration.Email;
using Klika.Identity.Model.Constants.IdentityConfig;
using Klika.Identity.Model.Entities;
using Klika.Identity.Model.Extensions.ModelStateMiddleware;
using Klika.Identity.Model.Interfaces.Mailer;
using Klika.Identity.Model.Interfaces.User;
using Klika.Identity.Service.Mailer;
using Klika.Identity.Service.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Klika.Identity.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private MapperConfiguration _mapperConfig { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => 
            { 
                options.AddPolicy("CORS", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); 
            });
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ValidateModelStateAttribute));
            });
            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                });

            services.AddAppVersioning();
            services.AddSwaggerConfiguration();
            services.AddAppInsights();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = Configuration.GetSection("AuthApiUrl").Value;
                    options.RequireHttpsMetadata = true;
                    options.Audience = InternalApis.IdentityServer;
                });
            
            services.AddDbContextPool<IdentityDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("IdentityDbContext")));

            services.AddSingleton(sp => _mapperConfig.CreateMapper());

            services.AddIdentityServerConfiguration(Configuration);
            services.AddScoped<IMailerService, MailerService>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();

            services.AddScoped<IIdentityUserService<ApplicationUser>, IdentityUserService<ApplicationUser>>();
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env, IdentityDbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                dbContext.Database.Migrate();
            }

            app.UseSwaggerConfiguration();
            app.UseAppInsights();
            app.UseCors("CORS");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseApiEndpoints();

            app.UseIdentityServer();
            await app.UseIdentityServerDataAsync(Configuration)
                        .ConfigureAwait(false);
        }
    }
}
