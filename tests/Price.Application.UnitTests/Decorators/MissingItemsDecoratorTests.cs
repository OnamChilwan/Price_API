using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Price.Application.Decorators;

namespace Price.Application.UnitTests.Decorators;

[TestFixture]
public class MissingItemsDecoratorTests
{
    private readonly ILogger<MissingItemsDecorator> _logger = Substitute.For<ILogger<MissingItemsDecorator>>();
    
    [Test]
    public async Task Given_Item_Does_Not_Exist_Then_Return_Item_Number_Only()
    {
        var subject = new MissingItemsDecorator(new MapItemPriceDecorator(), _logger);
        var entities = new EntityBuilder().WithItems("123456").Build();
        var context = DecoratorContext.Initialise(entities, new[] { "123456", "999999" }, "GBP", "gold");

        var result = await subject.Decorate(context);

        result.Should().HaveCount(2);
        result.First().ItemNumber.Should().Be("123456");
        result.First().Options.Should().BeEmpty();
        result.First().PriceHistory.Should().BeEmpty();
        
        result.ElementAt(1).ItemNumber.Should().Be("999999");
        result.ElementAt(1).Options.Should().BeEmpty();
        result.ElementAt(1).PriceHistory.Should().BeEmpty();
    }
    
    [Test]
    public async Task Given_Items_Exist_Then_All_Items_Are_Returned_With_Body()
    {
        var subject = new MissingItemsDecorator(new MapItemPriceDecorator(), _logger);
        var entities = new EntityBuilder().WithItems("123456").Build();
        var context = DecoratorContext.Initialise(entities, new[] { "123456", "123456" }, "GBP", "gold");

        var result = await subject.Decorate(context);

        result.Should().HaveCount(1);
        result.First().ItemNumber.Should().Be("123456");
        result.First().Options.Should().BeEmpty();
        result.First().PriceHistory.Should().BeEmpty();
    }
}