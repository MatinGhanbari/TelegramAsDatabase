using TelegramAsDatabase.Models;

namespace TelegramAsDatabase.Configs;

public class TDBConfig
{
    public string ApiKey { get; set; }
    public string ChannelId { get; set; }
    public TDBRetryPolicies RetryPolicies { get; set; } = new();
}