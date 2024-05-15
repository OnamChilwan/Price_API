using Bogus;
using Price.Infrastructure.Entities;

namespace Price.Api.ComponentTests.Fakes;

public sealed class OptionFaker : Faker<OptionEntity>
{
    public OptionFaker()
    {
        var salePricePeriodFaker = new SalePricePeriodFaker();
        var optionNumber = 1;
        RuleFor(x => x.OptionNumber, f => optionNumber++.ToString("00"));
        RuleFor(x => x.Price, f => decimal.Parse(f.Commerce.Price()));
        RuleFor(x => x.SalePricePeriods, f => salePricePeriodFaker.Generate(4));
    }
}