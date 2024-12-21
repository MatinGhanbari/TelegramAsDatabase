using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using TelegramAsDatabase.Configs;

namespace TelegramAsDatabase.Implementations;

internal class TDBTelegramBotClient : TelegramBotClient
{
    private readonly ILogger<TDBTelegramBotClient> _logger;
    private readonly TDBConfig _config;

    public TDBTelegramBotClient(IOptions<TDBConfig> configOptions, ILogger<TDBTelegramBotClient> logger, HttpClient? httpClient = null, CancellationToken cancellationToken = default)
        : base(configOptions.Value.ApiKey, httpClient, cancellationToken)
    {
        _config = configOptions.Value;
        _logger = logger;
    }

    public override async Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        return await Policy.Handle<Exception>()
                           .RetryAsync(_config.RetryPolicies.RetryCount, (e, i) => _logger.LogError($"{e.Message}, RetryNumber: {i}"))
                           .ExecuteAsync(async () => await base.SendRequest(request, cancellationToken));
    }
}