﻿using FluentResults;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TelegramAsDatabase.Configs;
using TelegramAsDatabase.Contracts;
using TelegramAsDatabase.Models;

namespace TelegramAsDatabase.Implementations;

public class TDB : ITDB
{
    private readonly TDBConfig _config;
    private readonly TelegramBotClient _bot;
    private int dbIndexMessageId;
    private TDBIndex tdbIndex;

    public TDB(IOptions<TDBConfig> configOptions)
    {
        _config = configOptions.Value;
        _bot = new TelegramBotClient(_config.ApiKey);

        ValidateConfig();
        SetIndexMessageId();
    }

    private void SetIndexMessageId()
    {
        try
        {
            var desc = _bot.GetMyDescription().Result;
            if (!string.IsNullOrEmpty(desc.Description))
            {
                dbIndexMessageId = Convert.ToInt32(desc.Description!);
                var message = _bot.ForwardMessage(_config.ChannelId, _config.ChannelId, dbIndexMessageId).Result;
                tdbIndex = message.Text!;
                _bot.DeleteMessage(_config.ChannelId, message.MessageId);
                return;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Cant get index message from bot description");
        }

        tdbIndex = new TDBIndex();
        var indexMessage = _bot.SendMessage(_config.ChannelId, (string)tdbIndex, ParseMode.Html).Result;
        _bot.SetMyDescription(indexMessage.MessageId.ToString());
        dbIndexMessageId = indexMessage.MessageId;
    }

    private void ValidateConfig()
    {
        try
        {
            _bot.GetMe().Wait();
        }
        catch (AggregateException exception)
        {
            throw new Exception("The bot api key is not valid!");
        }
    }

    public async Task<Result<TDBData<T>>> GetAsync<T>(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!tdbIndex.Data.TryGetValue(id, out var messageId)) return Result.Fail("Id does not exists");
            var message = await _bot.ForwardMessage(_config.ChannelId, _config.ChannelId, Convert.ToInt32(messageId), cancellationToken: cancellationToken);
            TDBData<T> data = message.Text!;
            await _bot.DeleteMessage(_config.ChannelId, message.MessageId, cancellationToken: cancellationToken);
            return data;
        }
        catch (Exception exception)
        {
            return Result.Fail(exception.Message);
        }
    }

    public async Task<Result<bool>> ExistsAsync<T>(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return tdbIndex.Data.TryGetValue(id, out _);
        }
        catch (Exception exception)
        {
            return Result.Fail(exception.Message);
        }
    }

    public async Task<Result> SaveAsync<T>(TDBData<T> data, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = await _bot.SendMessage(_config.ChannelId, data, ParseMode.Html, cancellationToken: cancellationToken);
            tdbIndex.Data.TryAdd(data.Id, message.MessageId.ToString());
            await _bot.EditMessageText(_config.ChannelId, dbIndexMessageId, tdbIndex, ParseMode.Html, cancellationToken: cancellationToken);
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
            tdbIndex = new TDBIndex();
            var indexMessage = await _bot.SendMessage(_config.ChannelId, (string)tdbIndex, ParseMode.Html, cancellationToken: cancellationToken);
            await _bot.SetMyDescription(indexMessage.MessageId.ToString(), cancellationToken: cancellationToken);
            dbIndexMessageId = indexMessage.MessageId;
            return Result.Ok();
        }
        catch (Exception exception)
        {
            return Result.Fail(exception.Message);
        }
    }
}