using Bogus;
using Price.Infrastructure.Entities;

namespace Price.Api.ComponentTests.Fakes;

public sealed class PriceFaker : Faker<PriceEntity>
{
    public PriceFaker()
    {
        RuleFor(x => x.MaxPrice, f => decimal.Parse(f.Commerce.Price(50, 100)));
        RuleFor(x => x.MinPrice, f => decimal.Parse(f.Commerce.Price(10, 49)));
    }
}