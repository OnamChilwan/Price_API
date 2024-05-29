using Dapr.Client;
using Microsoft.Extensions.Logging;
using Price.Infrastructure.Entities;
using Price.Infrastructure.Factories;

namespace Price.Infrastructure.Queries;

public interface IGetMultiplePricesQuery
{
    Task<IEnumerable<ItemPriceEntity>> Execute(IEnumerable<string> ids, string realm);
}

public class DaprGetMultiplePricesQuery : IGetMultiplePricesQuery
{
    private readonly DaprClient _client;
    private readonly ILogger<DaprGetMultiplePricesQuery> _logger;

    public DaprGetMultiplePricesQuery(DaprClient client, ILogger<DaprGetMultiplePricesQuery> logger)
    {
        _client = client;
        _logger = logger;
    }
    
    public async Task<IEnumerable<ItemPriceEntity>> Execute(IEnumerable<string> ids, string realm)
    {
        _logger.LogInformation("Hello world");
        
        //var foobar = string.Join(',', ids.Select(x => $"'{x}'"));
        //var query = $"{{ \"filter\": {{ \"IN\": {{ \"id\": [ {foobar} ] }} }} }}";
        var storeName = "statestore";
        var id = "next-gb-gold-123";
        var query = "{" +
                    "\"filter\": {" +
                    "\"EQ\": { \"c.id\": \"" + id + "\" }" +
                    "}}";

        // var e = new ItemPriceEntity() { Id = "next-gb-gold-999" };
        // await _client.SaveStateAsync<ItemPriceEntity>(storeName, e.Id, e);
        // _logger.LogInformation("Saved");
        
        var a = await _client.GetStateAsync<ItemPriceEntity>(storeName, "next-gb-gold-999");
        _logger.LogInformation($"Have A");
        
        var result = await _client.QueryStateAsync<ItemPriceEntity>(storeName, query);
        _logger.LogInformation($"Have Result {result.Results.Count}");
        
        if (a != null)
        {
            _logger.LogInformation($"A is not null {a.ItemNumber}");
            return new List<ItemPriceEntity>
            {
                new()
                {
                    Dataset = "GOLD",
                    Price = new PriceEntity { },
                    Id = a.Id,
                    Options = new List<OptionEntity> { new()
                    {
                        OptionNumber = "1",
                        Price = 2M
                    }},
                    Realm = "UK",
                    Territory = "GB",
                    ItemNumber = "123",
                    PriceHistory = new List<PriceHistoryEntity>(),
                    PriceGroupCode = "GB"
                }
            };
        }
        else
        {
            _logger.LogInformation($"A is null");
        }

        if (result.Results.Any())
        {
            return new List<ItemPriceEntity>();
        }

        return new List<ItemPriceEntity>();
    }
}

public class CosmosGetMultiplePricesQuery : IGetMultiplePricesQuery
{
    private readonly CosmosContainerFactory _cosmosContainerFactory;

    public CosmosGetMultiplePricesQuery(CosmosContainerFactory cosmosContainerFactory)
    {
        _cosmosContainerFactory = cosmosContainerFactory;
    }

    public async Task<IEnumerable<ItemPriceEntity>> Execute(IEnumerable<string> ids, string realm)
    {
        var container = _cosmosContainerFactory.Create(realm);
        var items = new List<ItemPriceEntity>();
        var sql = $"SELECT * FROM c WHERE c.id in ({string.Join(',', ids.Select(x => $"'{x}'"))})";
        using var feed = container.GetItemQueryIterator<ItemPriceEntity>(queryText: sql);

        while (feed.HasMoreResults)
        {
            var response = await feed.ReadNextAsync();
            items.AddRange(response);
        }

        return items;
    }
}

public class FakeGetMultiplePricesQuery : IGetMultiplePricesQuery
{
    public Task<IEnumerable<ItemPriceEntity>> Execute(IEnumerable<string> ids, string realm)
    {
        var items = new List<ItemPriceEntity>
        {
            new()
            {
                Dataset = "GOLD",
                Price = new PriceEntity { },
                Id = "id-foo",
                Options = new List<OptionEntity> { new()
                {
                    OptionNumber = "1",
                    Price = 2M
                }},
                Realm = "UK",
                Territory = "GB",
                ItemNumber = "123",
                PriceHistory = new List<PriceHistoryEntity>(),
                PriceGroupCode = "GB"
            }
        };
        return Task.FromResult<IEnumerable<ItemPriceEntity>>(items);
    }
}