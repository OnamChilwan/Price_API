using System.Collections.Generic;
using Bogus;
using Price.Infrastructure.Entities;

namespace Price.Api.ComponentTests.Fakes;

public sealed class ItemPriceFaker : Faker<ItemPriceEntity>
{
    public ItemPriceFaker(string id)
    {
        var priceFaker = new PriceFaker();
        
        RuleFor(x => x.Dataset, f => "gold");
        RuleFor(x => x.ItemNumber, f => id);
        RuleFor(x => x.Id, (f, i) => $"next-gold-gb-{id}");
        RuleFor(x => x.Options, new List<OptionEntity>());
        RuleFor(x => x.PriceGroupCode, f => "pgc");
        RuleFor(x => x.Price, f => priceFaker.Generate());
        RuleFor(x => x.PriceHistory, new List<PriceHistoryEntity>());
        RuleFor(x => x.Realm, f => "next");
        RuleFor(x => x.Territory, f => "gb");
    }
}