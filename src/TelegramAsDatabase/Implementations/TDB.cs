using FluentResults;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramAsDatabase.Configs;
using TelegramAsDatabase.Contracts;
using TelegramAsDatabase.Models;

namespace TelegramAsDatabase.Implementations;

public class TDB : ITDB, IDisposable
{
    private int _indexMessageId;
    private readonly TDBConfig _config;
    private Lazy<TDBKeyValueIndex> _tdbKeyValueIndex;
    private readonly ILogger<TDB> _logger;
    private IMemoryCache _memoryCache;
    private readonly TimeSpan _cacheExpiration;

    private readonly ITDBTelegramBotClient _bot;

    public TDB([FromKeyedServices(nameof(TDBTelegramBotClient))] ITelegramBotClient bot,
        IOptions<TDBConfig> configOptions,
        ILogger<TDB> logger,
        IMemoryCache memoryCache)
    {
        _bot = (ITDBTelegramBotClient?)bot;
        _logger = logger;
        _memoryCache = memoryCache;
        _config = configOptions.Value;
        _cacheExpiration = TimeSpan.FromSeconds(_config.CacheExpiration);

        ValidateBotClient(_bot);
        SetDefaultBotAdminRights(_bot);

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

    private void SetDefaultBotAdminRights(ITelegramBotClient bot)
    {
        bot.SetMyDefaultAdministratorRights(new ChatAdministratorRights()
        {
            CanPostMessages = true,
            CanEditMessages = true,
            CanDeleteMessages = true,
            CanPinMessages = true,
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
            throw new Exception("The bot api key is not valid!", exception);
        }
    }

    private TDBKeyValueIndex GetOrCreateIndex(int messageId = default)
    {
        try
        {
            if (messageId != default)
            {
                _indexMessageId = messageId;
                return _bot.GetAsync(_config.ChannelId, _indexMessageId).Result;
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.Message);
        }

        var tdbIndex = new TDBKeyValueIndex();

        var indexMessage = _bot.SaveAsync(_config.ChannelId, tdbIndex, ParseMode.Html).Result;
        Task.Run(() => _bot.SetMyDescription(indexMessage.MessageId.ToString()));
        Task.Run(() => _bot.PinChatMessage(_config.ChannelId, indexMessage.MessageId));
        _indexMessageId = indexMessage.MessageId;

        return tdbIndex;
    }

    private TDBKeyValueIndex RecreateIndex(int messageId = default)
    {
        var tdbIndex = new TDBKeyValueIndex();

        var indexMessage = _bot.SaveAsync(_config.ChannelId, tdbIndex, ParseMode.Html).Result;
        Task.Run(() => _bot.SetMyDescription(indexMessage.MessageId.ToString()));
        Task.Run(() => _bot.PinChatMessage(_config.ChannelId, indexMessage.MessageId));
        _indexMessageId = indexMessage.MessageId;

        return tdbIndex;
    }

    private async Task UpdateIndex(CancellationToken cancellationToken = default)
    {
        var msg = await _bot.UpdateAsync(_config.ChannelId, _indexMessageId, _tdbKeyValueIndex.Value, ParseMode.Html, cancellationToken: cancellationToken);
        _indexMessageId = msg.MessageId;
        Task.Run(() => _bot.SetMyDescription(_indexMessageId.ToString(), cancellationToken: cancellationToken), cancellationToken);
    }
    #endregion

    public async Task<Result<TDBData<T>>> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_tdbKeyValueIndex.Value.IndexIds.TryGetValue(key, out var messageId))
                return Result.Fail("key does not exists");

            if (_memoryCache.TryGetValue(key, out var memoryCacheResult) && memoryCacheResult != null)
                return Result.Ok((TDBData<T>)memoryCacheResult);

            var message = await _bot.GetAsync(_config.ChannelId, messageId, cancellationToken: cancellationToken);

            TDBData<T> data = message;

            _memoryCache.Set(key, data, _cacheExpiration);

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

            var message = await _bot.SaveAsync(_config.ChannelId, item, ParseMode.Html, cancellationToken: cancellationToken);

            _tdbKeyValueIndex.Value.IndexIds.TryAdd(item.Key, message.MessageId);

            _memoryCache.Set(item.Key, item, _cacheExpiration);

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
                item.Verify();

                var message = await _bot.SaveAsync(_config.ChannelId, item, ParseMode.Html, cancellationToken: cancellationToken);
                _tdbKeyValueIndex.Value.IndexIds.TryAdd(item.Key, message.MessageId);
                _memoryCache.Set(item.Key, item, _cacheExpiration);
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

            var message = await _bot.UpdateAsync(_config.ChannelId, messageId, value, ParseMode.Html, cancellationToken: cancellationToken);
            _tdbKeyValueIndex.Value.IndexIds[key] = message.MessageId;

            _memoryCache.Set(key, value, _cacheExpiration);

            UpdateIndex(cancellationToken);
            return Result.Ok();
        }
        catch (ApiRequestException exception) when (exception.Message.Contains("message is not modified", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning(exception.Message);
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

            _memoryCache.Remove(key);

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
                _memoryCache.Remove(key);

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
            Task.Run(() => _bot.DeleteMessages(_config.ChannelId,
                _tdbKeyValueIndex.Value.IndexIds
                    .Select(x => x.Value)
                    .ToList()
                , cancellationToken), cancellationToken);

            _tdbKeyValueIndex.Value.IndexIds.Clear();

            _memoryCache.Dispose();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            await UpdateIndex(cancellationToken);

            return Result.Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.Message);
            return Result.Fail(exception.Message);
        }
    }

    public async Task<Result<ITDBTransaction>> BeginTransaction(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_tdbKeyValueIndex.IsValueCreated)
            {
                var unused = _tdbKeyValueIndex.Value;
            }

            return Result.Ok((ITDBTransaction)new TDBTransaction(this, _indexMessageId));
        }
        catch (Exception exception)
        {
            return Result.Fail(exception.Message);
        }
    }

    internal async Task RecreateIndex()
    {
        var clone = _tdbKeyValueIndex.Value.Clone();

        _tdbKeyValueIndex = new Lazy<TDBKeyValueIndex>(() =>
        {
            try
            {
                var indexMessage = _bot.SaveAsync(_config.ChannelId, clone, ParseMode.Html).Result;
                Task.Run(() => _bot.SetMyDescription(indexMessage.MessageId.ToString()));
                Task.Run(() => _bot.PinChatMessage(_config.ChannelId, indexMessage.MessageId));
                _indexMessageId = indexMessage.MessageId;
                return clone;
            }
            finally
            {
                _logger.LogDebug("TDB KeyValueIndex loaded");
            }
        });
    }

    internal async Task RollbackIndex(int indexMessageId)
    {
        _tdbKeyValueIndex = new Lazy<TDBKeyValueIndex>(() =>
        {
            try
            {
                _indexMessageId = indexMessageId;
                return _bot.GetAsync(_config.ChannelId, _indexMessageId).Result;
            }
            finally
            {
                _logger.LogWarning("TDB KeyValueIndex Rollback");
            }
        });

        await UpdateIndex();
    }

    public void Dispose()
    {
        try
        {
            if (_tdbKeyValueIndex.IsValueCreated)
                UpdateIndex(CancellationToken.None).Wait();

            _memoryCache.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while disposing TDB instance.");
        }
        finally
        {
            _logger.LogInformation("The TDB instance was disposed!");
        }
    }
}