using FluentAssertions;
using Moq;
using NUnit.Framework;
using Price.Application.Decorators;
using Price.Application.Features;

namespace Price.Application.UnitTests.Decorators;

[TestFixture]
public class PriceHistoryDecoratorTests
{
    private Mock<IDecorator> _nextDecorator;
    private DecoratorContext _context;

    [SetUp]
    public void Setup()
    {
        _nextDecorator = new Mock<IDecorator>();
        var entities = new EntityBuilder().WithItems("123456").WithPriceHistory().Build();
        var itemNumbers = new[] { "123456" };
        _context = DecoratorContext.Initialise(entities, itemNumbers);
    }

    [Test]
    public async Task Sales_Is_Not_Enabled_Was_Price_Should_Not_Be_Populated()
    {
        var subject = new PriceHistoryDecorator(_nextDecorator.Object, SalesFeature.Create(false));
        var items = await subject.Decorate(_context);

        _nextDecorator.Verify(x => x.Decorate(_context), Times.Once);
        
        foreach (var itemPrice in items)
        {
            itemPrice.WasPrice.Should().BeNull();
        }
    }
    
    [Test]
    public async Task Sales_Is_Enabled_Was_Price_Should_Be_Populated()
    {
        var subject = new PriceHistoryDecorator(_nextDecorator.Object, SalesFeature.Create(true));
        var items = await subject.Decorate(_context);

        _nextDecorator.Verify(x => x.Decorate(_context), Times.Once);
        
        foreach (var itemPrice in items)
        {
            itemPrice.WasPrice.Should().NotBeNull();
        }
    }

    [Test]
    public async Task Sales_Is_Enabled_And_Item_Has_No_Price_History()
    {
        var subject = new PriceHistoryDecorator(_nextDecorator.Object, SalesFeature.Create(true));
        var context = DecoratorContext.Initialise(new EntityBuilder().WithItems("123456").Build(), new []{ "123456" });
        var items = await subject.Decorate(context);

        _nextDecorator.Verify(x => x.Decorate(context), Times.Once);
        
        foreach (var itemPrice in items)
        {
            itemPrice.WasPrice.Should().BeNull();
        }
    }
}