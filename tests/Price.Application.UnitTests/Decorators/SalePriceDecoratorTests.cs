using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Price.Application.Decorators;
using Price.Application.Features;

namespace Price.Application.UnitTests.Decorators;

[TestFixture]
public class SalePriceDecoratorTests
{
    private readonly ILogger<SalePriceDecorator> _logger = Substitute.For<ILogger<SalePriceDecorator>>();
    
    [Test]
    public async Task Given_Feature_Not_Enabled_When_Decorating_Then_Sale_Price_Is_Not_Populated()
    {
        var nextDecorator = Substitute.For<IDecorator>();
        var context = DecoratorContext.Initialise(new EntityBuilder()
            .WithItems("123456")
            .Build(), 
            new []{ "123456" },
            "GBP", "gold");
        
        var subject = new SalePriceDecorator(nextDecorator, SalesFeature.Default(), TimeMachineFeature.Default(), _logger);
        var result = await subject.Decorate(context);
        
        await nextDecorator.Received(1).Decorate(context);

        foreach (var itemPrice in result)
        {
            itemPrice.SalePrice.Should().BeNull();
        }
    }

    [Test]
    public async Task Given_Feature_Is_Enabled_And_No_Sales_Periods_Exist_When_Decorating_Then_Sale_Price_Is_Not_Populated()
    {
        var nextDecorator = Substitute.For<IDecorator>();
        var now = DateTime.UtcNow;
        var context = DecoratorContext.Initialise(new EntityBuilder()
            .WithItems("123456")
            .WithOptions()
            .WithNoActiveSalePeriods(now)
            .Build(), 
            new []{ "123456" }, "GBP", "gold");
        var subject = new SalePriceDecorator(nextDecorator, SalesFeature.Create(true), TimeMachineFeature.Default(), _logger);
        var result = await subject.Decorate(context);
        
        await nextDecorator.Received(1).Decorate(context);
        
        foreach (var itemPrice in result)
        {
            itemPrice.SalePrice.Should().BeNull();
        }
    }

    [Test]
    public async Task Given_Feature_Is_Enabled_And_Has_An_Active_Sale_When_Decorating_Then_Sale_Price_Is_Populated()
    {
        var context = DecoratorContext.Initialise(new EntityBuilder()
            .WithItems("123456")
            .WithOptions()
            .WithSingleActiveSalePeriod(out var minPrice, out var maxPrice)
            .Build(),
            new []{ "123456" }, "GBP", "gold");
        
        var subject = new SalePriceDecorator(new MapItemPriceDecorator(), SalesFeature.Create(true), TimeMachineFeature.Default(), _logger);
        var result = await subject.Decorate(context);
        
        result.First().SalePrice.Should().NotBeNull();
        result.First().SalePrice.MinPrice.Should().Be(minPrice);
        result.First().SalePrice.MaxPrice.Should().Be(maxPrice);
    }

    [Test]
    public async Task Given_Feature_Is_Enabled_And_Has_Multiple_Active_Sales_Across_Different_Options_When_Decorating_Then_Sale_Price_Is_Populated()
    {
        var context = DecoratorContext.Initialise(new EntityBuilder()
            .WithItems("111111", "222222", "333333")
            .WithOptions()
            .WithMultipleActiveSalePeriods(out var minPrice, out var maxPrice)
            .Build(),
            new []{ "111111", "222222", "333333" }, "GBP", "gold");
        var subject = new SalePriceDecorator(new MapItemPriceDecorator(), SalesFeature.Create(true), TimeMachineFeature.Default(), _logger);
        var result = await subject.Decorate(context);
        
        result.First().SalePrice.Should().NotBeNull();
        result.First().SalePrice.MinPrice.Should().Be(minPrice);
        result.First().SalePrice.MaxPrice.Should().Be(maxPrice);
    }
}