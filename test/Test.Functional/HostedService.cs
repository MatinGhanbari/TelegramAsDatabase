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
        var id = Guid.NewGuid();
        Console.WriteLine($"Id: {id}");

        var model = new MyTestModel()
        {
            Name = "FirstTest",
            Description = "FirstDescription",
            Type = "FuncTest"
        };

        var result = await _tdb.SaveAsync(new TDBData<MyTestModel>()
        {
            Id = id,
            Data = model
        }, cancellationToken);

        var exists = await _tdb.ExistsAsync<MyTestModel>(id, cancellationToken);
        Console.WriteLine($"ModelExists: {exists.Value}");

        exists = await _tdb.ExistsAsync<MyTestModel>(Guid.NewGuid(), cancellationToken);
        Console.WriteLine($"ModelExists: {exists.Value}");

        var modelResult = await _tdb.GetAsync<MyTestModel>(id, cancellationToken);
        var newModel = modelResult.Value.Data;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("####### Stopped #######");
    }
}