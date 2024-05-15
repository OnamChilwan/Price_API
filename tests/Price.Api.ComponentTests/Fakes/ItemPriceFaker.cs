using System.Linq;
using Bogus;
using Price.Infrastructure.Entities;

namespace Price.Api.ComponentTests.Fakes;

public sealed class ItemPriceFaker : Faker<ItemPriceEntity>
{
    public ItemPriceFaker()
    {
        var optionFaker = new OptionFaker();
        var priceFaker = new PriceFaker();
        var priceHistoryFaker = new PriceHistoryFaker();
        
        var itemNumber = 1;
        RuleFor(x => x.Dataset, f => "gold");
        RuleFor(x => x.ItemNumber, f => itemNumber++.ToString());
        RuleFor(x => x.Id, (f, i) => $"next-gold-gb-{i.ItemNumber}");
        RuleFor(x => x.Options, f => optionFaker.Generate(2).ToList());
        RuleFor(x => x.PriceGroupCode, f => "pgc");
        RuleFor(x => x.Price, f => priceFaker.Generate());
        RuleFor(x => x.PriceHistory, priceHistoryFaker.Generate(4));
        RuleFor(x => x.Realm, f => "next");
        RuleFor(x => x.Territory, f => "gb");
    }
}