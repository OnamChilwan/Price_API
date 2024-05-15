using Bogus;
using Price.Infrastructure.Entities;

namespace Price.Api.ComponentTests.Fakes;

public sealed class PriceHistoryFaker : Faker<PriceHistoryEntity>
{
    public PriceHistoryFaker()
    {
        RuleFor(x => x.MaxPrice, f => f.Random.Decimal(100, 200));
        RuleFor(x => x.MinPrice, f => f.Random.Decimal(50, 99));
        RuleFor(x => x.DatePoint, f => f.Date.Past());
    }
}