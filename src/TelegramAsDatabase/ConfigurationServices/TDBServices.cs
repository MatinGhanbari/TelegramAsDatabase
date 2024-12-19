using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using Telegram.Bot;
using TelegramAsDatabase.Configs;
using TelegramAsDatabase.Contracts;
using TelegramAsDatabase.Implementations;

namespace TelegramAsDatabase.ConfigurationServices;

public static class TDBServices
{
    public static IServiceCollection AddTDB(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TDBConfig>(configuration.GetSection(nameof(TDBConfig)));

        services.AddKeyedSingleton<ITelegramBotClient, TelegramBotClient>(nameof(TDB), (serviceProvider, _) =>
        {
            var configOptions = serviceProvider.GetRequiredService<IOptions<TDBConfig>>();
            var apiKey = configOptions.Value.ApiKey;
            return new TelegramBotClient(apiKey);
        });

        services.AddSingleton<ITDB, TDB>(serviceProvider =>
        {
            var configOptions = serviceProvider.GetRequiredService<IOptions<TDBConfig>>();
            var client = serviceProvider.GetKeyedService<ITelegramBotClient>(nameof(TDB));
            return new TDB(configOptions, client);
        });
        return services;
    }
}