using Bogus;
using Price.Infrastructure.Entities;

namespace Price.Api.ComponentTests.Fakes;

public sealed class SalePricePeriodFaker : Faker<SalePricePeriodEntity>
{
    public SalePricePeriodFaker(bool activeSale)
    {
        RuleFor(x => x.SalePrice, f => decimal.Parse(f.Commerce.Price()));
        RuleFor(x => x.PriceActiveFrom, f => activeSale ? f.Date.Past() : f.Date.Soon());
        RuleFor(x => x.PriceActiveTo, f => f.Date.Future());
    }
}