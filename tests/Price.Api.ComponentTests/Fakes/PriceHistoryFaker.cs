using Bogus;
using Price.Infrastructure.Entities;

namespace Price.Api.ComponentTests.Fakes;

public sealed class PriceHistoryFaker : Faker<PriceHistoryEntity>
{
    public PriceHistoryFaker(decimal? minPrice, decimal? maxPrice)
    {
        RuleFor(x => x.MinPrice, f => minPrice ?? decimal.Parse(f.Commerce.Price(50, 99)));
        RuleFor(x => x.MaxPrice, f => maxPrice ?? decimal.Parse(f.Commerce.Price(100, 200)));
        RuleFor(x => x.DatePoint, f => f.Date.Past().Date);
    }
}