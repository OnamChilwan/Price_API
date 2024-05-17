using System.Linq;
using NUnit.Framework;
using Price.Api.ComponentTests.Builders;
using Price.Api.ComponentTests.Steps;
using TestStack.BDDfy;

namespace Price.Api.ComponentTests.Fixtures;

public class GetMultipleItemPricesTests
{
    private readonly MultipleItemPriceSteps _steps = new();

    [Test]
    public void Given_Sales_Feature_Is_Disabled_Then_Items_Are_Returned_Without_Sale_Price()
    {
        var items = new ItemPriceBuilder()
            .WithItems("ABC123", "DEF456")
            .WithPriceHistory("ABC123", "DEF456")
            .WithOptions("ABC123", "01", "02")
            .WithActiveSalePeriod("ABC123", "01", out _, out _)
            .Build()
            .ToArray();
        
        this.Given(s => _steps.GivenSalesFeatureIs(false))
            .And(s => _steps.FollowingItemsExist("next", "gb", items))
            .When(s => _steps.ValidRequestIsSentToMultiplePrices("next", "gb", "en", items))
            .Then(s => _steps.CorrectHttpResponseCodeIsReturned())
            .And(s => _steps.MultipleItemPricesAreReturned(items))
            .And(s => _steps.SaleInformationIsNotPresent())
            .BDDfy();
    }
    
    [Test]
    public void Given_Sales_Feature_Is_Enabled_And_An_Active_Sale_Period_Then_Items_Are_Returned_With_Sale_History_And_Sale_Prices()
    {
        var items = new ItemPriceBuilder()
            .WithItems("ABC123", "DEF456")
            .WithPriceHistory("ABC123", "DEF456")
            .WithOptions("ABC123", "01", "02")
            .WithActiveSalePeriod("ABC123", "01", out var saleItem, out var salePrice)
            .WithInactiveSalePeriod("ABC123", "01")
            .Build()
            .ToArray();
        
        this.Given(s => _steps.GivenSalesFeatureIs(true))
            .And(s => _steps.FollowingItemsExist("next", "gb", items))
            .When(s => _steps.ValidRequestIsSentToMultiplePrices("next", "gb", "en", items))
            .Then(s => _steps.CorrectHttpResponseCodeIsReturned())
            .And(s => _steps.MultipleItemPricesAreReturned(items))
            .And(s => _steps.SaleInformationIsPresent(saleItem!, salePrice))
            .BDDfy();
    }

    [Test]
    public void Given_Sale_Feature_Is_Enabled_And_No_Active_Sale_Period_Then_Items_Are_Returned_Without_Sale_Price()
    {
        var items = new ItemPriceBuilder()
            .WithItems("ABC123", "DEF456")
            .WithPriceHistory("ABC123", "DEF456")
            .WithOptions("ABC123", "01", "02")
            .WithInactiveSalePeriod("ABC123", "01")
            .Build()
            .ToArray();
        
        this.Given(s => _steps.GivenSalesFeatureIs(true))
            .And(s => _steps.FollowingItemsExist("next", "gb", items))
            .When(s => _steps.ValidRequestIsSentToMultiplePrices("next", "gb", "en", items))
            .Then(s => _steps.CorrectHttpResponseCodeIsReturned())
            .And(s => _steps.MultipleItemPricesAreReturned(items))
            .BDDfy();
    }
}