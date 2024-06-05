using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Price.Application.Decorators;
using Price.Application.Features;

namespace Price.Application.UnitTests.Decorators;

[TestFixture]
public class PriceHistoryDecoratorTests
{
    private IDecorator _nextDecorator = null!;
    private DecoratorContext _context = null!;

    [SetUp]
    public void Setup()
    {
        _nextDecorator = Substitute.For<IDecorator>();
        var entities = new EntityBuilder().WithItems("123456").WithPriceHistory().Build();
        var itemNumbers = new[] { "123456" };
        _context = DecoratorContext.Initialise(entities, itemNumbers, "GBP", "gold");
    }

    [Test]
    public async Task Sales_Is_Not_Enabled_Was_Price_Should_Not_Be_Populated()
    {
        var subject = new PriceHistoryDecorator(_nextDecorator, SalesFeature.Create(false));
        var items = await subject.Decorate(_context);

        await _nextDecorator.Received(1).Decorate(_context);
        
        foreach (var itemPrice in items)
        {
            itemPrice.WasPrice.Should().BeNull();
        }
    }
    
    [Test]
    public async Task Sales_Is_Enabled_Was_Price_Should_Be_Populated()
    {
        var subject = new PriceHistoryDecorator(_nextDecorator, SalesFeature.Create(true));
        var items = await subject.Decorate(_context);

        await _nextDecorator.Received(1).Decorate(_context);
        
        foreach (var itemPrice in items)
        {
            itemPrice.WasPrice.Should().NotBeNull();
        }
    }

    [Test]
    public async Task Sales_Is_Enabled_And_Item_Has_No_Price_History()
    {
        var subject = new PriceHistoryDecorator(_nextDecorator, SalesFeature.Create(true));
        var items = await subject.Decorate(_context);

        await _nextDecorator.Received(1).Decorate(_context);
        
        foreach (var itemPrice in items)
        {
            itemPrice.WasPrice.Should().BeNull();
        }
    }
}