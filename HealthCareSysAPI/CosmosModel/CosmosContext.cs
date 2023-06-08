using HealthCareSysAPI.CosmosModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.Net;
using System;
using System.Threading.Tasks;
using HealthCareSysAPI.Custom_Classes;

public class CosmosContext
{
    private readonly CosmosClient _cosmosClient;
    private readonly Database _database;
    private readonly Container _container;
    private static readonly List<Action<Chat>> _messageCallbacks = new List<Action<Chat>>();


    // Replace "your_connection_string_here" with your actual Azure Cosmos DB connection string
    private const string DatabaseName = "ChatDatabase";
    private const string ContainerName = "ChatContainer";

    public CosmosContext(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
        _database = _cosmosClient.GetDatabase(DatabaseName);
        _container = _database.GetContainer(ContainerName);
    }

    // Add methods to interact with the Cosmos DB container, such as CRUD operations

    // Example method to retrieve chat messages
    public async Task AddChatMessageAsync(Chat message)
    {
        await _container.CreateItemAsync(message, new PartitionKey(message.receiver));
    }

    public async Task<List<Chat>> GetChatMessagesByReceiverAsync(string receiver)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.receiver = @receiver")
            .WithParameter("@receiver", receiver);

        var messages = new List<Chat>();
        var queryIterator = _container.GetItemQueryIterator<Chat>(query);

        while (queryIterator.HasMoreResults)
        {
            var response = await queryIterator.ReadNextAsync();
            messages.AddRange(response.ToList());
        }

        return messages;
    }
    public async Task<List<Chat>> GetFullConversation(string receiver , string sender)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE (c.receiver = '" + receiver + "' and c.sender = '" + sender + "') or (c.receiver = '"+sender+"' and c.sender = '"+receiver+"' )");
            

        var messages = new List<Chat>();
        var queryIterator = _container.GetItemQueryIterator<Chat>(query);

        while (queryIterator.HasMoreResults)
        {
            var response = await queryIterator.ReadNextAsync();
            messages.AddRange(response.ToList());
        }

        return messages;
    }
    public async Task<dynamic> SendMessage(Chat message)
    {
        RandomID random = new RandomID();

        dynamic document = new
        {
            id = random.GenerateRandomId(25),
            content = message.content,
            sender = message.sender,
            receiver = message.receiver,
            timestamp = DateTime.Now
        };
        var response = await _container.CreateItemAsync(document);
       return response;
    }
    public void RegisterMessageCallback(Action<Chat> callback)
    {
        _messageCallbacks.Add(callback);
    }
    public void UnRegisterMessageCallback(Action<Chat> callback)
    {
        _messageCallbacks.Remove(callback);
    }

    public void NotifyNewMessage(Chat message)
    {
        foreach (var callback in _messageCallbacks)
        {
            callback.Invoke(message);
        }
    }
}
