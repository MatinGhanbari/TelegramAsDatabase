using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TelegramAsDatabase.ConfigurationServices;
using Test.Functional;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((hostingContext, configuration) =>
            {
                configuration.AddJsonFile(path: "appsettings.json", optional: false, true);
            })
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;

                services.AddHostedService<HostedService>();

                // Add other services
                services.AddTDB(configuration);
            });
}