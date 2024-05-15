using Bogus;
using Price.Infrastructure.Entities;

namespace Price.Api.ComponentTests.Fakes;

public sealed class PriceFaker : Faker<PriceEntity>
{
    public PriceFaker()
    {
        RuleFor(x => x.MaxPrice, f => f.Random.Decimal(50, 100));
        RuleFor(x => x.MinPrice, f => f.Random.Decimal(10, 49));
    }
}