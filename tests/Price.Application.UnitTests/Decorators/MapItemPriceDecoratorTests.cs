using FluentAssertions;
using NUnit.Framework;
using Price.Application.Decorators;
using Price.Application.DTOs;
using Price.Infrastructure.Entities;

namespace Price.Application.UnitTests.Decorators;

[TestFixture]
public class MapItemPriceDecoratorTests
{
    private MapItemPriceDecorator _subject;

    [SetUp]
    public void Setup()
    {
        _subject = new MapItemPriceDecorator();
    }

    [Test]
    public async Task Given_There_Are_Item_Prices_When_Mapping_Then_Items_Are_Mapped_Correctly()
    {
        var context = DecoratorContext.Initialise(new EntityBuilder()
            .WithItems("123456")
            .WithPriceHistory()
            .WithOptions()
            .Build(),
            new []{ "123456" });
        
        var items = await _subject.Decorate(context);
        var result = items.First();
        
        result.Dataset.Should().Be(context.Entities.First().Dataset);
        result.Id.Should().Be(context.Entities.First().Id);
        result.Realm.Should().Be(context.Entities.First().Realm);
        result.Territory.Should().Be(context.Entities.First().Territory);
        result.ItemNumber.Should().Be("123456");
        result.Price.MaxPrice.Should().Be(context.Entities.First().Price.MaxPrice);
        result.Price.MinPrice.Should().Be(context.Entities.First().Price.MinPrice);
        
        result.PriceHistory.Should().NotBeEmpty();
        AssertPriceHistory(result.PriceHistory.First(), context.Entities.First().PriceHistory!.First());
        AssertPriceHistory(result.PriceHistory.ElementAt(1), context.Entities.First().PriceHistory!.ElementAt(1));
        
        result.Options.Should().NotBeEmpty();
        AssertOption(result.Options.First(), context.Entities.First().Options.First());
        AssertOption(result.Options.ElementAt(1), context.Entities.First().Options.ElementAt(1));
    }

    [Test]
    public async Task Given_There_Are_No_Options_When_Mapping_Options_Are_Empty()
    {
        var context = DecoratorContext.Initialise(new EntityBuilder()
                .WithItems("123456")
                .WithPriceHistory()
                .Build(),
            new []{ "123456" });
        var items = await _subject.Decorate(context);
        var result = items.First();

        result.Options.Should().BeEmpty();
    }
    
    [Test]
    public async Task Given_There_Is_No_Price_History_When_Mapping_History_Is_Empty()
    {
        var context = DecoratorContext.Initialise(new EntityBuilder()
            .WithItems("123456")
            .WithOptions()
            .Build(),
            new []{ "123456" });
        var items = await _subject.Decorate(context);
        var result = items.First();

        result.PriceHistory.Should().BeEmpty();
    }

    [Test]
    [Ignore("Find out what output should be")]
    public async Task Given_There_Are_No_Items_Then_Empty_Object_Is_Returned()
    {
        var context = DecoratorContext.Initialise(new EntityBuilder().Build(), Enumerable.Empty<string>());
        var items = await _subject.Decorate(context);
        var result = items.First();

        result.Dataset.Should().BeNull();
        result.Id.Should().Be(context.Entities.First().Id);
        result.Realm.Should().BeNull();
        result.Territory.Should().BeNull();
        result.ItemNumber.Should().BeNull();
        result.Price.Should().BeNull();
        
        result.PriceHistory.Should().BeEmpty();
        result.Options.Should().BeEmpty();
    }

    private static void AssertPriceHistory(PriceHistoryDto subject, PriceHistoryEntity assertion)
    {
        subject.DatePoint.Should().Be(assertion.DatePoint);
        subject.MaxPrice.Should().Be(assertion.MaxPrice);
        subject.MinPrice.Should().Be(assertion.MinPrice);
    }
    
    private static void AssertOption(OptionDto subject, OptionEntity assertion)
    {
        subject.Price.Should().Be(assertion.Price);
        subject.OptionNumber.Should().Be(assertion.OptionNumber);
        subject.IsSalePrice.Should().BeFalse();
    }
}