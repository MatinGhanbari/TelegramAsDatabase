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
        //await _tdb.ClearAsync(cancellationToken);

        var modelResult = await _tdb.GetAsync<MyTestModel>(Guid.Parse("07ead21e-fd48-4b91-a11b-f313bca898fa"), cancellationToken);
        var newModel = modelResult.Value.Data;

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

         modelResult = await _tdb.GetAsync<MyTestModel>(id, cancellationToken);
         newModel = modelResult.Value.Data;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("####### Stopped #######");
    }
}