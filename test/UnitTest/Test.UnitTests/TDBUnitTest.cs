using TelegramAsDatabase.Models;
using Test.UnitTests.Builders;
using Test.UnitTests.Models;

namespace Test.UnitTests;

public class TDBUnitTest
{
    [Fact]
    public async void SaveAsync_WithValidInput_ReturnsOk()
    {
        var tdb = new TDBBuilder().Build();
        var user = UserBuilder.Build();

        var results = await tdb.SaveAsync(new TDBData<User>()
        {
            Key = user.Id.ToString(),
            Value = user
        });

        Assert.True(results.IsSuccess);
        Assert.False(results.IsFailed);
        Assert.Empty(results.Errors);
    }
}