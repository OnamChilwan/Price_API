using NSubstitute;
using NUnit.Framework;
using Price.Application.Decorators;
using Price.Application.Services;
using Price.Infrastructure.Entities;
using Price.Infrastructure.Queries;

namespace Price.Application.UnitTests.Services;

public class PriceApplicationServiceTests
{
    [Test]
    public async Task When()
    {
        var query = Substitute.For<IGetMultiplePricesQuery>();
        var decorator = Substitute.For<IDecorator>();
        var subject = new PriceApplicationService(query, decorator);
        
        query
            .Execute(Arg.Any<IEnumerable<string>>(), Arg.Any<string>())
            .Returns(new List<ItemPriceEntity> { new() { Dataset = "afsafaf" }});

        await subject.GetMultiplePrices("", "", "", new[] { "123" });
    }
}