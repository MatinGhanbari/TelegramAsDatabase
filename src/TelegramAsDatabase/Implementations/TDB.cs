using FluentResults;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TelegramAsDatabase.Configs;
using TelegramAsDatabase.Contracts;
using TelegramAsDatabase.Models;

namespace TelegramAsDatabase.Implementations;

public class TDB : ITDB
{
    private int _indexMessageId;
    private readonly TDBConfig _config;
    private readonly Lazy<TDBKeyValueIndex> _tdbKeyValueIndex;
    private readonly ILogger<TDB> _logger;

    private readonly ITelegramBotClient _bot;

    public TDB(IOptions<TDBConfig> configOptions, [FromKeyedServices(nameof(TDB))] ITelegramBotClient bot, ILogger<TDB> logger)
    {
        ValidateBotClient(bot);

        _bot = bot;
        _logger = logger;
        _config = configOptions.Value;

        _tdbKeyValueIndex = new Lazy<TDBKeyValueIndex>(() =>
        {
            try
            {
                var botDescription = _bot.GetMyDescription()?.Result?.Description ?? "";
                int.TryParse(botDescription, out var indexMessageId);
                return GetOrCreateIndex(indexMessageId);
            }
            finally
            {
                _logger.LogDebug("TDB KeyValueIndex loaded");
            }
        });
    }

    #region [- Private Methods -]
    private void ValidateBotClient(ITelegramBotClient bot)
    {
        try
        {
            bot.GetMe().Wait();
        }
        catch (AggregateException exception)
        {
            _logger.LogError("TDB bot api key validation failed!");
            throw new Exception("The bot api key is not valid!");
        }
    }

    private TDBKeyValueIndex GetOrCreateIndex(int messageId = default)
    {
        if (messageId != default)
        {
            _indexMessageId = messageId;

            var message = _bot.ForwardMessage(_config.ChannelId, _config.ChannelId, _indexMessageId).Result;

            Task.Run(() => _bot.DeleteMessage(_config.ChannelId, message.MessageId));
            return message.Text!;
        }

        var tdbIndex = new TDBKeyValueIndex();

        var indexMessage = _bot.SendMessage(_config.ChannelId, tdbIndex, ParseMode.Html).Result;
        Task.Run(() => _bot.SetMyDescription(indexMessage.MessageId.ToString()));
        _indexMessageId = indexMessage.MessageId;

        return tdbIndex;
    }

    private async Task<string> GetMessageText(int messageId, bool deleteForwardedMessage = true, CancellationToken cancellationToken = default)
    {
        var message = await _bot.ForwardMessage(_config.ChannelId, _config.ChannelId, messageId, cancellationToken: cancellationToken);

        if (deleteForwardedMessage)
            Task.Run(async () => await _bot.DeleteMessage(_config.ChannelId, message.MessageId, cancellationToken), cancellationToken);

        return message.Text;
    }

    private async Task UpdateIndex(CancellationToken cancellationToken = default)
    {
        Task.Run(async () => await _bot.EditMessageText(_config.ChannelId, _indexMessageId, _tdbKeyValueIndex.Value, ParseMode.Html, cancellationToken: cancellationToken), cancellationToken);
    }
    #endregion

    public async Task<Result<TDBData<T>>> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_tdbKeyValueIndex.Value.IndexIds.TryGetValue(key, out var messageId))
                return Result.Fail("key does not exists");

            var message = await GetMessageText(messageId, cancellationToken: cancellationToken);

            TDBData<T> data = message;
            return data;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.Message);
            return Result.Fail(exception.Message);
        }
    }

    public async Task<Result<List<string>>> GetAllKeysAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return _tdbKeyValueIndex.Value.IndexIds.Select(x => x.Key).ToList();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.Message);
            return Result.Fail(exception.Message);
        }
    }

    public async Task<Result<bool>> ExistsAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return _tdbKeyValueIndex.Value.IndexIds.TryGetValue(key, out _);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.Message);
            return Result.Fail(exception.Message);
        }
    }

    public async Task<Result> SaveAsync<T>(TDBData<T> item, CancellationToken cancellationToken = default)
    {
        try
        {
            item.Verify();

            var message = await _bot.SendMessage(_config.ChannelId, item, ParseMode.Html, cancellationToken: cancellationToken);

            _tdbKeyValueIndex.Value.IndexIds.TryAdd(item.Key, message.MessageId);

            UpdateIndex(cancellationToken);
            return Result.Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.Message);
            return Result.Fail(exception.Message);
        }
    }

    public async Task<Result> SaveAsync<T>(IEnumerable<TDBData<T>> items, CancellationToken cancellationToken = default)
    {
        try
        {
            foreach (var item in items)
            {
                var message = await _bot.SendMessage(_config.ChannelId, item, ParseMode.Html, cancellationToken: cancellationToken);
                _tdbKeyValueIndex.Value.IndexIds.TryAdd(item.Key, message.MessageId);
            }

            UpdateIndex(cancellationToken);
            return Result.Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.Message);
            return Result.Fail(exception.Message);
        }
    }

    public async Task<Result> UpdateAsync<T>(string key, TDBData<T> value, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_tdbKeyValueIndex.Value.IndexIds.TryGetValue(key, out var messageId))
                return Result.Fail("key does not exists");

            var message = await _bot.EditMessageText(_config.ChannelId, messageId, value, ParseMode.Html, cancellationToken: cancellationToken);
            return Result.Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.Message);
            return Result.Fail(exception.Message);
        }
    }

    public async Task<Result> DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_tdbKeyValueIndex.Value.IndexIds.Remove(key, out var messageId))
                return Result.Fail("key does not exists");

            UpdateIndex(cancellationToken);
            Task.Run(async () => await _bot.DeleteMessage(_config.ChannelId, messageId, cancellationToken), cancellationToken);

            return Result.Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.Message);
            return Result.Fail(exception.Message);
        }
    }

    public async Task<Result> DeleteAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        try
        {
            var keysList = keys.ToList();
            var messageIds = new List<int>();

            foreach (var key in keysList)
            {
                if (_tdbKeyValueIndex.Value.IndexIds.Remove(key, out var messageId))
                    messageIds.Add(messageId);
            }

            Task.Run(async () => await _bot.DeleteMessages(_config.ChannelId, messageIds, cancellationToken), cancellationToken);

            UpdateIndex(cancellationToken);
            return Result.Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.Message);
            return Result.Fail(exception.Message);
        }
    }

    public async Task<Result> ClearAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _tdbKeyValueIndex.Value.IndexIds.Clear();
            await UpdateIndex(cancellationToken);

            return Result.Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.Message);
            return Result.Fail(exception.Message);
        }
    }
}