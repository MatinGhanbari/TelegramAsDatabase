using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramAsDatabase.Contracts;

public interface ITDBTelegramBotClient : ITelegramBotClient
{
    Task<Message> SaveAsync(
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
       CancellationToken cancellationToken = default);

    Task<Message> UpdateAsync(
        ChatId chatId,
        int messageId,
        string text,
        ParseMode parseMode = ParseMode.None,
        IEnumerable<MessageEntity>? entities = null,
        LinkPreviewOptions? linkPreviewOptions = null,
        InlineKeyboardMarkup? replyMarkup = null,
        string? businessConnectionId = null,
        CancellationToken cancellationToken = default(CancellationToken));

    Task<string> GetAsync(
        ChatId chatId,
        int messageId,
        CancellationToken cancellationToken = default);
}