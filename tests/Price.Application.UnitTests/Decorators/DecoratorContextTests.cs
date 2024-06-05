using FluentAssertions;
using NUnit.Framework;
using Price.Application.Decorators;

namespace Price.Application.UnitTests.Decorators;

[TestFixture]
public class DecoratorContextTests
{
    [Test]
    public void Given_Valid_Item_Numbers_And_Entities_Then_Context_Is_Initialised()
    {
        var entities = new EntityBuilder()
            .WithItems("123456")
            .WithPriceHistory()
            .WithOptions()
            .Build();
        var itemNumbers = new[] { "1", "2" };
        var subject = DecoratorContext.Initialise(entities, itemNumbers, "GBP", "gold");

        subject.RequestedItemNumbers.Should().BeEquivalentTo(itemNumbers);
        subject.Entities.Should().BeEquivalentTo(entities);
        subject.Currency.Should().Be("GBP");
        subject.Dataset.Should().Be("gold");
    }
}