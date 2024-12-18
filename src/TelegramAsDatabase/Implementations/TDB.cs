﻿using FluentResults;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Microsoft.Extensions.Options;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TelegramAsDatabase.Configs;
using TelegramAsDatabase.Contracts;
using TelegramAsDatabase.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TelegramAsDatabase.Implementations;

public class TDB : ITDB
{
    private int _indexMessageId;
    private readonly TDBConfig _config;
    private readonly TDBKeyValueIndex _tdbKeyValueIndex;

    private readonly ITelegramBotClient _bot;

    public TDB(IOptions<TDBConfig> configOptions)
    {
        _config = configOptions.Value;
        _bot = new TelegramBotClient(_config.ApiKey);

        ValidateBotClient(_bot);

        var botDescription = _bot.GetMyDescription().Result.Description;
        int.TryParse(botDescription, out var indexMessageId);
        _tdbKeyValueIndex = GetOrCreateIndex(indexMessageId);
    }

    private void ValidateBotClient(ITelegramBotClient bot)
    {
        try
        {
            bot.GetMe().Wait();
        }
        catch (AggregateException exception)
        {
            throw new Exception("The bot api key is not valid!");
        }
    }

    private TDBKeyValueIndex GetOrCreateIndex(int messageId = default)
    {
        if (messageId != default)
        {
            _indexMessageId = (int)messageId;

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

    public async Task<Result<TDBData<T>>> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_tdbKeyValueIndex.IndexIds.TryGetValue(key, out var messageId))
                return Result.Fail("key does not exists");

            var message = await GetMessageText(messageId, cancellationToken: cancellationToken);

            TDBData<T> data = message;
            return data;
        }
        catch (Exception exception)
        {
            return Result.Fail(exception.Message);
        }
    }

    public async Task<Result<bool>> ExistsAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return _tdbKeyValueIndex.IndexIds.TryGetValue(key, out _);
        }
        catch (Exception exception)
        {
            return Result.Fail(exception.Message);
        }
    }

    public async Task<Result> SaveAsync<T>(TDBData<T> item, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = await _bot.SendMessage(_config.ChannelId, item, ParseMode.Html, cancellationToken: cancellationToken);

            _tdbKeyValueIndex.IndexIds.TryAdd(item.Key, message.MessageId);

            UpdateIndex(cancellationToken);
            return Result.Ok();
        }
        catch (Exception exception)
        {
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
                _tdbKeyValueIndex.IndexIds.TryAdd(item.Key, message.MessageId);
            }

            UpdateIndex(cancellationToken);
            return Result.Ok();
        }
        catch (Exception exception)
        {
            return Result.Fail(exception.Message);
        }
    }

    public async Task<Result> ClearAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _tdbKeyValueIndex.IndexIds.Clear();
            await UpdateIndex(cancellationToken);

            return Result.Ok();
        }
        catch (Exception exception)
        {
            return Result.Fail(exception.Message);
        }
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
        Task.Run(async () => await _bot.EditMessageText(_config.ChannelId, _indexMessageId, _tdbKeyValueIndex, ParseMode.Html, cancellationToken: cancellationToken), cancellationToken);
    }
}