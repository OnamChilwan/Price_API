using Microsoft.Azure.Cosmos;

namespace Price.Infrastructure.Factories;

public class CosmosContainerFactory
{
    private readonly CosmosClient _cosmosClient;
    private readonly string _databaseId;

    public CosmosContainerFactory(CosmosClient cosmosClient, string databaseId)
    {
        _cosmosClient = cosmosClient;
        _databaseId = databaseId;
    }
    
    public Container Create(string realm)
    {
        if (realm.Contains("next", StringComparison.CurrentCultureIgnoreCase))
        {
            return _cosmosClient.GetContainer(_databaseId, "ItemPrice-Next");
        }
        
        return _cosmosClient.GetContainer(_databaseId, "ItemPrice-TP");
    }
}