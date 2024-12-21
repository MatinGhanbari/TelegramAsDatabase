using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramAsDatabase;

internal class TDBConstants
{
    internal const int MaxMessageLength = 4096;

    internal const int MaxGroupMessagesPerMinute = 20;
    internal const int MaxChatMessagesPerSecond = 1;

    internal static readonly char[] MarkdownCharacters = ['_', '*', '[', ']', '(', ')', '~', '`', '>', '#', '+', '-', '=', '|', '{', '}', '.', '!'];
}