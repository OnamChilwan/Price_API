using Price.Infrastructure.Entities;
using Price.Infrastructure.Factories;

namespace Price.Infrastructure.Queries;

public interface IGetMultiplePricesQuery
{
    Task<IEnumerable<ItemPriceEntity>> Execute(IEnumerable<string> ids, string realm);
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