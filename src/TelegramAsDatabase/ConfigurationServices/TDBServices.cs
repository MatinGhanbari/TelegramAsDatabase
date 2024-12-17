using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TelegramAsDatabase.Configs;
using TelegramAsDatabase.Contracts;
using TelegramAsDatabase.Implementations;

namespace TelegramAsDatabase.ConfigurationServices;

public static class TDBServices
{
    public static IServiceCollection AddTDB(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TDBConfig>(configuration.GetSection(nameof(TDBConfig)));
        services.AddSingleton<ITDB, TDB>(serviceProvider =>
        {
            var configOptions = serviceProvider.GetRequiredService<IOptions<TDBConfig>>();
            return new TDB(configOptions);
        });
        return services;
    }
}