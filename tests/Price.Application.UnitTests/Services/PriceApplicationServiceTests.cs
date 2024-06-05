using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Price.Application.Decorators;
using Price.Application.DTOs;
using Price.Application.Services;
using Price.Infrastructure.Entities;
using Price.Infrastructure.Queries;

namespace Price.Application.UnitTests.Services;

public class PriceApplicationServiceTests
{
    [Test]
    public async Task Given_Valid_Request_Then_Items_Are_Returned_Successfully()
    {
        var query = Substitute.For<IGetMultiplePricesQuery>();
        var decorator = Substitute.For<IDecorator>();
        var subject = new PriceApplicationService(query, decorator, Substitute.For<ILogger<PriceApplicationService>>());
        
        const string realm = "realm";
        const string territory = "territory";
        const string currency = "GBP";
        const string dataset = "gold";
        var itemNumbers = new[] { "123" };
        var entities = new List<ItemPriceEntity>();
        var items = new List<ItemPriceDto>();
        
        query
            .Execute(Arg.Is<IEnumerable<string>>(arr => IsEquivalentTo(arr, itemNumbers)), Arg.Is(realm))
            .Returns(entities);

        decorator
            .Decorate(Arg.Is<DecoratorContext>(x => 
                x.Currency == currency &&
                x.Dataset == dataset &&
                x.RequestedItemNumbers == itemNumbers &&
                x.Entities == entities))
            .Returns(items);
        
        var result = await subject.GetMultiplePrices(realm, territory, currency, dataset, itemNumbers);

        result.Should().BeEquivalentTo(items);
    }
    
    private static bool IsEquivalentTo(IEnumerable<string> x, IEnumerable<string> ids)
    {
        x.Should().BeEquivalentTo(ids);
        return true;
    }
}