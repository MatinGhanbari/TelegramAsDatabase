# TDB: Telegram as database service
This project is a .NET application that utilizes Telegram's free cloud storage to provide a simple, scalable database solution. It enables data storage and management through a Telegram bot, allowing users to interact with the database directly via Telegram. By leveraging Telegram's cloud infrastructure, the project eliminates the need for traditional database setups, offering a lightweight and efficient alternative for small to medium-scale data storage needs.

## Features

1. **Asynchronous Operations:** All methods are asynchronous (using `Task`) to avoid blocking the calling thread.
1. **Transaction Support:** Offers the ability to begin a transaction for atomic operations.
1. **Generic Interface:** The interface is generic (`ITDB`) allowing it to work with various data types.
1. **Data Model:** Uses a generic data model (`TDBData<T>`) to encapsulate data and metadata.
1. **FluentResults Integration:** Leverages the FluentResults library for robust error handling and reporting.
1. **Cancellation Support:** All methods accept an optional `CancellationToken` for graceful cancellation.
1. **Key-Value Storage:** Primarily designed for key-value based data storage and retrieval.
1. **Basic CRUD Operations:** Supports common operations like Get, Set, Update, Delete, and Exists.
1. **Bulk Operations:** Includes methods for saving and deleting multiple items in bulk.
1. **Key Retrieval:** Provides a method to retrieve all keys currently stored.
1. **Retry Policy:** Implements a retry policy to enhance resilience against transient failures. This policy can be configured to retry operations a specified number of times with optional backoff strategies, improving the overall reliability and availability of the service.

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
        "ApiKey": "7753344678:AAFf4MIQSShxa4djp172DjhDe2_jqRsyOeU",
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
public class MyCustomService
{
    private ITDB _tdbService;

    public MyCustomService(ITDB tdb)
    {
        _tdbService = tdb;
    }

    public async Task MyCustomMethod(CancellationToken cancellationToken)
    {
        // [--------- GetAllKeysAsync ---------]
        var allKeys = await _tdbService.GetAllKeysAsync(cancellationToken);

        // [------------ SaveAsync ------------]
        var saveResult = await _tdbService.SaveAsync(new TDBData<MyTestModel>()
        {
            Key = "itemkey",
            Value = new MyTestModel()
            {
                Name = "FirstTest",
                Description = "FirstDescription",
                Type = "FuncTest"
            }
        }, cancellationToken);

        // [--------- UpdateAsync ---------]
        var updateResult = await _tdbService.UpdateAsync("itemkey", new TDBData<MyTestModel>()
        {
            Key = "itemkey",
            Value = new MyTestModel()
            {
                Name = "FirstTest2",
                Description = "FirstDescription2",
                Type = "FuncTest2"
            }
        }, cancellationToken);

        // [--------- DeleteAsync ---------]
        var deleteResult = await _tdbService.DeleteAsync("itemkey", cancellationToken);
    }
}
```
