using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

        services.AddKeyedSingleton<ITelegramBotClient, TDBTelegramBotClient>(nameof(TDBTelegramBotClient), (serviceProvider, _) =>
        {
            var configOptions = serviceProvider.GetRequiredService<IOptions<TDBConfig>>();
            var logger = serviceProvider.GetRequiredService<ILogger<TDBTelegramBotClient>>();
            return new TDBTelegramBotClient(configOptions, logger);
        });

        services.AddSingleton<ITDB, TDB>(serviceProvider =>
        {
            var configOptions = serviceProvider.GetRequiredService<IOptions<TDBConfig>>();
            var client = serviceProvider.GetKeyedService<ITelegramBotClient>(nameof(TDBTelegramBotClient));
            var logger = serviceProvider.GetRequiredService<ILogger<TDB>>();
            return new TDB(client, configOptions, logger);
        });
        return services;
    }

    public static IServiceCollection AddTDB(this IServiceCollection services, Action<TDBServiceConfiguration> configuration)
    {
        var serviceConfig = new TDBServiceConfiguration();
        configuration.Invoke(serviceConfig);
        return services.AddTDB(serviceConfig);
    }

    private static IServiceCollection AddTDB(this IServiceCollection services, TDBServiceConfiguration configuration)
    {
        var config = configuration.Config;

        services.AddKeyedSingleton<ITelegramBotClient, TDBTelegramBotClient>(nameof(TDBTelegramBotClient), (serviceProvider, _) =>
        {
            IOptions<TDBConfig> configOptions = new OptionsWrapper<TDBConfig>(config);
            var logger = serviceProvider.GetRequiredService<ILogger<TDBTelegramBotClient>>();
            return new TDBTelegramBotClient(configOptions, logger);
        });

        services.AddSingleton<ITDB, TDB>(serviceProvider =>
        {
            IOptions<TDBConfig> configOptions = new OptionsWrapper<TDBConfig>(config);
            var client = serviceProvider.GetKeyedService<ITelegramBotClient>(nameof(TDBTelegramBotClient));
            var logger = serviceProvider.GetRequiredService<ILogger<TDB>>();
            return new TDB(client, configOptions, logger);
        });
        return services;
    }
}