using FluentAssertions;
using NUnit.Framework;
using Price.Application.Decorators;

namespace Price.Application.UnitTests.Decorators;

[TestFixture]
public class MissingItemsDecoratorTests
{
    [Test]
    public async Task Given_Item_Does_Not_Exist_Then_Return_Item_Number_Only()
    {
        var subject = new MissingItemsDecorator(new MapItemPriceDecorator());
        var entities = new EntityBuilder().WithItems("123456").Build();
        var context = DecoratorContext.Initialise(entities, new[] { "123456", "999999" });

        var result = await subject.Decorate(context);

        result.Should().HaveCount(2);
        result.First().ItemNumber.Should().Be("123456");
        result.First().Options.Should().BeEmpty();
        result.First().PriceHistory.Should().BeEmpty();
        
        result.ElementAt(1).ItemNumber.Should().Be("999999");
        result.ElementAt(1).Options.Should().BeNull();
        result.ElementAt(1).PriceHistory.Should().BeNull();
    }
    
    [Test]
    public async Task Given_Items_Exist_Then_All_Items_Are_Returned_With_Body()
    {
        var subject = new MissingItemsDecorator(new MapItemPriceDecorator());
        var entities = new EntityBuilder().WithItems("123456").Build();
        var context = DecoratorContext.Initialise(entities, new[] { "123456", "123456" });

        var result = await subject.Decorate(context);

        result.Should().HaveCount(1);
        result.First().ItemNumber.Should().Be("123456");
        result.First().Options.Should().BeEmpty();
        result.First().PriceHistory.Should().BeEmpty();
    }
}