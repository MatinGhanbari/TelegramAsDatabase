using Bogus;
using Microsoft.Extensions.Options;
using Moq;
using Telegram.Bot;
using TelegramAsDatabase.Configs;
using TelegramAsDatabase.Contracts;
using TelegramAsDatabase.Implementations;

namespace Test.UnitTests.Builders;

public class TDBBuilder
{
    private static readonly Faker Faker = new("en");

    private readonly Mock<ITelegramBotClient> _botClientMock;
    private readonly Mock<IOptions<TDBConfig>> _configMock;

    public TDBBuilder()
    {
        _botClientMock = new();
        _configMock = new();
    }

    public ITDB Build()
    {
        // Todo: Setup Mock
        //_botClientMock.Setup(client =>
        //    client.SendMessage(
        //        It.IsAny<ChatId>(),
        //        It.IsAny<string>(),
        //        It.IsAny<ParseMode>(),
        //        It.IsAny<ReplyParameters?>(),
        //        It.IsAny<IReplyMarkup?>(),
        //        It.IsAny<LinkPreviewOptions?>(),
        //        It.IsAny<int?>(),
        //        It.IsAny<IEnumerable<MessageEntity>?>(),
        //        It.IsAny<bool>(),
        //        It.IsAny<bool>(),
        //        It.IsAny<string?>(),
        //        It.IsAny<string?>(),
        //        It.IsAny<bool>(),
        //        It.IsAny<CancellationToken>()
        //    )
        //).Returns(Task.FromResult(new Message() { Id = Faker.Random.Int() }));

        //_botClientMock.Setup(client =>
        //    client.ForwardMessage(
        //        It.IsAny<int>(),
        //        It.IsAny<int>(),
        //        It.IsAny<int>(),
        //        It.IsAny<int?>(),
        //        It.IsAny<bool>(),
        //        It.IsAny<bool>(),
        //        It.IsAny<CancellationToken>()
        //        )
        //    ).ReturnsAsync(new Message() { Id = Faker.Random.Int() });

        //_botClientMock.Setup(client =>
        //    client.DeleteMessage(
        //        It.IsAny<int>(),
        //        It.IsAny<int>(),
        //        It.IsAny<CancellationToken>()
        //        )
        //);

        return new TDB(_configMock.Object, _botClientMock.Object);
    }
}