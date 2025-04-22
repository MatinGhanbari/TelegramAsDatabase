using TelegramAsDatabase.Models;
using Test.UnitTests.Builders;
using Test.UnitTests.Models;

namespace Test.Units.UnitTests;

public class TDBGetAsyncUnitTest
{
    [Fact]
    public async void GetAsync_WithValidInput_ReturnsOk()
    {
        var tdb = new TDBBuilder().Build();
        var user = new UserBuilder().With(builder => builder.Id, 100)
                                    .With(builder => builder.Name, "Matin")
                                    .Build();

        await tdb.SaveAsync(new TDBData<User> { Key = user.Id.ToString(), Value = user });

        var results = await tdb.GetAsync<User>(user.Id.ToString());

        Assert.True(results.IsSuccess);
        Assert.False(results.IsFailed);
        Assert.Empty(results.Errors);
    }


    [Fact]
    public async void GetAsync_WithValidInput2_ReturnsOk()
    {
        var tdb = new TDBBuilder().Build();
        var user = new UserBuilder().With(builder => builder.Id, 100)
            .With(builder => builder.Name, "Jess")
            .Build();

        await tdb.SaveAsync(new TDBData<User> { Key = user.Id.ToString(), Value = user });

        var results = await tdb.GetAsync<User>(user.Id.ToString());

        Assert.True(results.IsSuccess);
        Assert.False(results.IsFailed);
        Assert.Empty(results.Errors);
    }

    [Fact]
    public async void GetAsync_WithEmptyKey_ReturnsFailure()
    {
        var tdb = new TDBBuilder().Build();

        var results = await tdb.GetAsync<User>(string.Empty);

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
    public async void GetAsync_WithInvalidKey_ReturnsFailure(string value)
    {
        var tdb = new TDBBuilder().Build();

        var results = await tdb.GetAsync<User>(value);

        Assert.True(results.IsFailed);
        Assert.False(results.IsSuccess);
        Assert.NotEmpty(results.Errors);
    }
}