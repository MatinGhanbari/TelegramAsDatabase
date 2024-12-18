using Microsoft.Extensions.Hosting;
using TelegramAsDatabase.Contracts;
using TelegramAsDatabase.Models;
using Test.Functional.Models;

namespace Test.Functional;

internal class HostedService : IHostedService
{
    private readonly ITDB _tdb;
    public HostedService(ITDB tdb)
    {
        _tdb = tdb;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var key = "1";

        var model = new MyTestModel()
        {
            Name = "FirstTest2",
            Description = "FirstDescription",
            Type = "FuncTest"
        };

        var result = await _tdb.SaveAsync(new TDBData<MyTestModel>()
        {
            Key = key,
            Value = model
        }, cancellationToken);

        var exists = await _tdb.ExistsAsync<MyTestModel>(key, cancellationToken);
        Console.WriteLine($"ModelExists: {exists.Value}");

        exists = await _tdb.ExistsAsync<MyTestModel>("2", cancellationToken);
        Console.WriteLine($"ModelExists: {exists.Value}");

        var modelResult = await _tdb.GetAsync<MyTestModel>(key, cancellationToken);
        var newModel = modelResult.Value.Value;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("####### Stopped #######");
    }
}