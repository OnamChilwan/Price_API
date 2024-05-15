using Bogus;
using Price.Infrastructure.Entities;

namespace Price.Api.ComponentTests.Fakes;

public sealed class SalePricePeriodFaker : Faker<SalePricePeriodEntity>
{
    public SalePricePeriodFaker()
    {
        RuleFor(x => x.SalePrice, f => f.Random.Decimal(10, 30));
        RuleFor(x => x.PriceActiveFrom, f => f.Date.Past());
        RuleFor(x => x.PriceActiveTo, f => f.Date.Future());
    }
}