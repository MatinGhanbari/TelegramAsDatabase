# TelegramAsDatabase
The official repository for telegram as database
### Now telegram as database (TDB) is availabe on nuget.org: [click here](https://www.nuget.org/packages/TelegramAsDatabase)

# Getting start
1. Create a bot using bot father in telegram (Important: **Dont Change the bot description at all**)

    ![botfather](https://raw.githubusercontent.com/MatinGhanbari/TelegramAsDatabase/refs/heads/main/assets/images/botfather.png)
2. Create a channel as a database source in telegram
    
    ![channel](https://raw.githubusercontent.com/MatinGhanbari/TelegramAsDatabase/refs/heads/main/assets/images/channel.png)
3. Now add your bot that created in the first step to the channel as an administrator and give all premissions to it.
    
    ![channel](https://raw.githubusercontent.com/MatinGhanbari/TelegramAsDatabase/refs/heads/main/assets/images/adminrights.png)
4. Goto your appsettings and add TDB config to it.:
    - The api key is key that you get from BotFather in the first step
    - The Channel Id is the id of the channel you can get it by forwarding a message of the channel to @userinfobot bot.
    ```json
    "TDBConfig": {
        // The api key that you get from BotFather in the first step
        "ApiKey": "7753344678:AAFf4MIQSShxa4djp172DjhDe2_jqRsyOeU",
        // The Channel Id
        "ChannelId": "-1002378130994"
    }
    ```
5. Now Add Tdb to your service container:

    ```csharp
    var configurations = builder.Configuration;
    builder.Services.AddTDB(configurations);
    ```

6. Done! now you can use the telegram free database

# How to use
The example code of using the TDB:
```csharp
internal class TestService
{
    private readonly ITDB _tdb;
    public TestService(ITDB tdb)
    {
        _tdb = tdb;
    }

    public async Task StartService(CancellationToken cancellationToken)
    {
        // This line clears all data from db
        //await _tdb.ClearAsync(cancellationToken);

        var id = Guid.NewGuid();

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
       
         modelResult = await _tdb.GetAsync<MyTestModel>(id, cancellationToken);
         var data = modelResult.Value.Data;

         Console.WriteLine(data.Name); // output: FirstTest
    }
}
```
