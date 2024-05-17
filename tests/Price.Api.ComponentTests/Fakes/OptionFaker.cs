using System.Collections.Generic;
using Bogus;
using Price.Infrastructure.Entities;

namespace Price.Api.ComponentTests.Fakes;

public sealed class OptionFaker : Faker<OptionEntity>
{
    public OptionFaker(string optionNumber)
    {
        RuleFor(x => x.OptionNumber, f => optionNumber);
        RuleFor(x => x.Price, f => decimal.Parse(f.Commerce.Price()));
        RuleFor(x => x.SalePricePeriods, new List<SalePricePeriodEntity>());
    }
}