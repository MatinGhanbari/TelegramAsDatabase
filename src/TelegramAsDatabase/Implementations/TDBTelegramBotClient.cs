using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramAsDatabase.Configs;
using TelegramAsDatabase.Contracts;

namespace TelegramAsDatabase.Implementations;

public class TDBTelegramBotClient : TelegramBotClient, ITDBTelegramBotClient
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
        var policy = Policy
            .Handle<Exception>(e => !e.Message.Contains("message is not modified", StringComparison.OrdinalIgnoreCase)) // Exclude exceptions with "Duplicated Id"
            .RetryAsync(_config.RetryPolicies.RetryCount, (e, i) =>
                _logger.LogError($"{e.Message}, RetryNumber: {i}")
            );

        return await policy.ExecuteAsync(async () => await base.SendRequest(request, cancellationToken));
    }

    public async Task<Message> SaveAsync(
     ChatId chatId,
     string text,
     ParseMode parseMode = ParseMode.None,
     ReplyParameters? replyParameters = null,
     IReplyMarkup? replyMarkup = null,
     LinkPreviewOptions? linkPreviewOptions = null,
     int? messageThreadId = null,
     IEnumerable<MessageEntity>? entities = null,
     bool disableNotification = false,
     bool protectContent = false,
     string? messageEffectId = null,
     string? businessConnectionId = null,
     bool allowPaidBroadcast = false,
     CancellationToken cancellationToken = default)
    {
        if (text.Length < TDBConstants.MaxMessageLength)
        {
            return await this.SendMessage(chatId, text, parseMode, replyParameters,
                replyMarkup, linkPreviewOptions, messageThreadId, entities,
                disableNotification, protectContent,
                messageEffectId, businessConnectionId,
                allowPaidBroadcast, cancellationToken);
        }

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        var document = InputFile.FromStream(stream);

        return await this.SendDocument(chatId, document, null, parseMode, replyParameters, replyMarkup, null, messageThreadId, null, false, disableNotification, false, messageEffectId, businessConnectionId, allowPaidBroadcast, cancellationToken);
    }

    public async Task<Message> UpdateAsync(ChatId chatId, int messageId, string text,
        ParseMode parseMode = ParseMode.None, IEnumerable<MessageEntity>? entities = null, LinkPreviewOptions? linkPreviewOptions = null,
        InlineKeyboardMarkup? replyMarkup = null, string? businessConnectionId = null,
        CancellationToken cancellationToken = default)
    {
        Task.Run(async () => await this.DeleteMessage(_config.ChannelId, messageId, cancellationToken), cancellationToken);
        return await SaveAsync(chatId, text, parseMode, null, replyMarkup, linkPreviewOptions, cancellationToken: cancellationToken);
    }

    public async Task<string> GetAsync(
     ChatId chatId,
     int messageId,
     CancellationToken cancellationToken = default)
    {
        var message = await this.ForwardMessage(chatId, _config.ChannelId, messageId, cancellationToken: cancellationToken);
        Task.Run(async () => await this.DeleteMessage(_config.ChannelId, message.MessageId, cancellationToken), cancellationToken);

        if (message.Type == MessageType.Text)
        {
            return message.Text;
        }

        if (message.Type == MessageType.Document)
        {
            var stream = new MemoryStream();
            var fileName = message.Document.FileName;
            var fileInfo = await this.GetInfoAndDownloadFile(message.Document.FileId, stream, cancellationToken);

            var buffer = stream.ToArray();
            return Encoding.UTF8.GetString(buffer);
        }

        throw new Exception("Unexpected message");
    }
}