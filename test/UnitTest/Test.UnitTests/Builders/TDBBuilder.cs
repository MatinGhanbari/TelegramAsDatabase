using Bogus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using TelegramAsDatabase.Configs;
using TelegramAsDatabase.Contracts;
using TelegramAsDatabase.Implementations;

namespace Test.UnitTests.Builders;

public class TDBBuilder
{
    private static readonly Faker Faker = new("en");

    private readonly Mock<ITelegramBotClient> _botClientMock;
    private IOptions<TDBConfig> _configMock;

    public TDBBuilder()
    {
        _botClientMock = new();
    }

    public ITDB Build()
    {
        _configMock = new OptionsWrapper<TDBConfig>(new TDBConfig()
        {
            ChannelId = "-1000000",
            ApiKey = "RANDOM_API_KEY"
        });

        var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.AddConsole());

        return new TDB(new MockTelegramBotClient(), _configMock, loggerFactory.CreateLogger<TDB>());
    }
}

public class MockClientOptions
{
    public bool HandleNegativeOffset { get; set; }
    public string[] Messages { get; set; } = [];
    public int RequestDelay { get; set; } = 10;
    public Exception? ExceptionToThrow { get; set; }
    public CancellationToken GlobalCancelToken { get; set; }

}

public class MockTelegramBotClient : ITelegramBotClient
{
    private readonly Queue<string[]> _messages;

    public int? lastOffsetRequested = null;
    public int MessageGroupsLeft => _messages.Count;
    public MockClientOptions Options { get; }

    public Task<TResponse> MakeRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        => SendRequest(request, cancellationToken);
    public Task<TResponse> MakeRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        => SendRequest(request, cancellationToken);

    public async Task<TResponse> SendRequest<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        switch (request)
        {
            case SendMessageRequest sendMessageRequest:
            case DeleteMessageRequest deleteMessageRequest:
            case ForwardMessageRequest forwardMessageRequest:
                return (TResponse)(object)new Message
                {
                    Id = 100,
                    Text = JsonConvert.SerializeObject(new User())
                };

            default:
                return default;
        }
    }

    public TimeSpan Timeout { get; set; } = TimeSpan.FromMilliseconds(50);

    public IExceptionParser ExceptionsParser { get; set; } = new DefaultExceptionParser();

    // ---------------
    // NOT IMPLEMENTED
    // ---------------

    public bool LocalBotServer => throw new NotImplementedException();
    public long BotId => throw new NotImplementedException();
    public event AsyncEventHandler<ApiRequestEventArgs>? OnMakingApiRequest;
    public event AsyncEventHandler<ApiResponseEventArgs>? OnApiResponseReceived;
    public Task DownloadFile(
        string filePath,
        Stream destination,
        CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();
    public Task<bool> TestApi(CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();
}