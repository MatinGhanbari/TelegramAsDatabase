using TelegramAsDatabase.Models;
using Test.UnitTests.Builders;
using Test.UnitTests.Models;

namespace Test.Units.UnitTests;

public class TDBSaveAsyncUnitTest
{
    [Fact]
    public async void SaveAsync_WithValidInput_ReturnsOk()
    {
        var tdb = new TDBBuilder().Build();
        var user = new UserBuilder().With(builder => builder.Id, 100)
                                    .With(builder => builder.Name, "Matin")
                                    .Build();

        var results = await tdb.SaveAsync(new TDBData<User>()
        {
            Key = user.Id.ToString(),
            Value = user
        });

        Assert.True(results.IsSuccess);
        Assert.False(results.IsFailed);
        Assert.Empty(results.Errors);
    }

    [Fact]
    public async void SaveAsync_WithNullValue_ReturnsFailure()
    {
        var tdb = new TDBBuilder().Build();
        User user = null!;

        var results = await tdb.SaveAsync(new TDBData<User>()
        {
            Key = string.Empty,
            Value = user!
        });

        Assert.True(results.IsFailed);
        Assert.False(results.IsSuccess);
        Assert.NotEmpty(results.Errors);
    }

    [Fact]
    public async void SaveAsync_WithEmptyKey_ReturnsFailure()
    {
        var tdb = new TDBBuilder().Build();
        var user = new UserBuilder().Build();

        var results = await tdb.SaveAsync(new TDBData<User>()
        {
            Key = string.Empty,
            Value = user
        });

        Assert.True(results.IsFailed);
        Assert.False(results.IsSuccess);
        Assert.NotEmpty(results.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData("_")]
    [InlineData("*")]
    [InlineData("[")]
    [InlineData("]")]
    [InlineData("(")]
    [InlineData(")")]
    [InlineData("~")]
    [InlineData("`")]
    [InlineData(">")]
    [InlineData("#")]
    [InlineData("+")]
    [InlineData("-")]
    [InlineData("=")]
    [InlineData("|")]
    [InlineData("{")]
    [InlineData("}")]
    [InlineData(".")]
    [InlineData("!")]
    [InlineData(null)]
    public async void SaveAsync_WithInvalidKey_ReturnsFailure(string value)
    {
        var tdb = new TDBBuilder().Build();
        var user = new UserBuilder().Build();

        var results = await tdb.SaveAsync(new TDBData<User>()
        {
            Key = value,
            Value = user
        });

        Assert.True(results.IsFailed);
        Assert.False(results.IsSuccess);
        Assert.NotEmpty(results.Errors);
    }
}