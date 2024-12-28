using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TelegramAsDatabase.ConfigurationServices;

namespace TelegramAsDatabase.Samples;

internal class Program
{
    public static async Task Main(string[] args)
    {
        ThreadPool.SetMaxThreads(16, 16);
        ThreadPool.SetMinThreads(1, 1);
        await CreateHostBuilder(args).Build().RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
                   .ConfigureServices((hostBuilderContext, services) =>
                   {
                       services.AddTDB(sc =>
                       {
                           sc.Config.ApiKey = "7770450595:AAFtIQGYGHPD-CnqSoWkgQAJSYP8nnVYSSs";
                           sc.Config.ChannelId = "-1002305327746";
                           sc.Config.RetryPolicies.RetryCount = 2;
                       });
                       services.AddHostedService<MyService>();
                   });
    }
}