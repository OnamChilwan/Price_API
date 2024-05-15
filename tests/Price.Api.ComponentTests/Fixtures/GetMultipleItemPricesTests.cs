using NUnit.Framework;
using Price.Api.ComponentTests.Fakes;
using Price.Api.ComponentTests.Steps;
using TestStack.BDDfy;

namespace Price.Api.ComponentTests.Fixtures;

public class GetMultipleItemPricesTests
{
    private readonly MultipleItemPriceSteps _steps = new();

    [Test]
    public void Given_Sales_Feature_Is_Disabled_Then_Items_Are_Returned_Without_Sale_Price()
    {
        var faker = new ItemPriceFaker();
        var items = faker.Generate(2).ToArray();
        
        this.Given(s => _steps.GivenSalesFeatureIs(false))
            .And(s => _steps.FollowingItemsExist("next", "gb", items))
            .When(s => _steps.ValidRequestIsSentToMultiplePrices("next", "gb", "en", items))
            .Then(s => _steps.CorrectHttpResponseCodeIsReturned())
            .And(s => _steps.MultipleItemPricesAreReturned(items))
            .And(s => _steps.SaleInformationIsNotPresent())
            .BDDfy();
    }
    
    [Test]
    public void Given_Sales_Feature_Is_Enabled_Then_Items_Are_Returned_With_Sale_History_And_Sale_Prices()
    {
        var faker = new ItemPriceFaker();
        var items = faker.Generate(2).ToArray();
        
        this.Given(s => _steps.GivenSalesFeatureIs(true))
            .And(s => _steps.FollowingItemsExist("next", "gb", items))
            .When(s => _steps.ValidRequestIsSentToMultiplePrices("next", "gb", "en", items))
            .Then(s => _steps.CorrectHttpResponseCodeIsReturned())
            .And(s => _steps.MultipleItemPricesAreReturned(items))
            .And(s => _steps.SaleInformationIsPresent())
            .BDDfy();
    }

    public void Given_Sale_Feature_Is_Enabled_And_No_Active_Sale_Period_Then_Items_Are_Returned_Without_Sale_Price()
    {
        // TODO: complete this
    }
}