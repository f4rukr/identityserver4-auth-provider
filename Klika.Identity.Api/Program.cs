using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Klika.Identity.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
                    {
                        var currentEnv = hostingContext.HostingEnvironment;
                        configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                            .AddJsonFile($"appsettings.{currentEnv.EnvironmentName}.json", optional: true)
                                            .AddEnvironmentVariables();
                    });
                });
    }
}
