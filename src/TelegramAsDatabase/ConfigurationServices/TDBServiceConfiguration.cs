using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramAsDatabase.Configs;

namespace TelegramAsDatabase.ConfigurationServices;

public class TDBServiceConfiguration
{
    public TDBConfig Config { get; set; } = new();

    public void SetChannelId(string channelId) => Config.ChannelId = channelId;
    public void SetApiKey(string apiKey) => Config.ApiKey = apiKey;
}