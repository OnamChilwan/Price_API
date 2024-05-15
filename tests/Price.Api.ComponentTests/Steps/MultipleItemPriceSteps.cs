using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Price.Api.Middleware;
using Price.Api.Models.Responses;
using Price.Application.Features;
using Price.Infrastructure.Entities;
using Price.Infrastructure.Queries;

namespace Price.Api.ComponentTests.Steps;

public class MultipleItemPriceSteps
{
    private readonly TestServer _server = new(new WebHostBuilder().UseStartup<TestStartup>());
    private HttpResponseMessage _httpResponse = null!;
    private IEnumerable<ItemPrice> _result = null!;

    public void GivenSalesFeatureIs(bool featureFlag)
    {
        _server.Services
            .GetRequiredService<IFeatureFlagRequestContext>()
            .GetValue(Arg.Is(SalesFeatureMiddleware.FlagName))
            .Returns(featureFlag);
    }

    public void FollowingItemsExist(string realm, string territory, params ItemPriceEntity[] entities)
    {
        var ids = entities.Select(x => $"{realm}-{territory}-gold-{x.ItemNumber}");
        
        _server.Services
            .GetRequiredService<IGetMultiplePricesQuery>()
            .Execute(Arg.Is<IEnumerable<string>>(arr => IsEquivalentTo(arr, ids)), Arg.Is(realm))
            .Returns(entities);
    }
    
    public async Task ValidRequestIsSentToMultiplePrices(string realm, string territory, string language, params ItemPriceEntity[] entities)
    {
        var client = _server.CreateClient();
        var query = string.Join('&', entities.Select(x => $"itemNumber={x.ItemNumber}"));
        _httpResponse = await client.GetAsync($"/price/{realm}/{territory}/{language}/v1/prices?{query}");
    }

    public async Task CorrectHttpResponseCodeIsReturned()
    {
        _httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        _result = await _httpResponse.Content.ReadFromJsonAsync<IEnumerable<ItemPrice>>() ?? Enumerable.Empty<ItemPrice>();
    }

    public void MultipleItemPricesAreReturned(params ItemPriceEntity[] entities)
    {
        _result.Should().NotBeEmpty();
        entities.Should().NotBeEmpty();

        foreach (var entity in entities)
        {
            var item = _result.Single(x => x.ItemNumber.Equals(entity.ItemNumber));
            
            item.Id.Should().Be(entity.Id);
            item.ItemNumber.Should().Be(entity.ItemNumber);
            item.Realm.Should().Be(entity.Realm);
            item.Territory.Should().Be(entity.Territory);
            item.Dataset.Should().Be(entity.Dataset);
            // item.CurrencyCode.Should().Be(entity)

            item.Price.MinPrice.Should().Be(entity.Price.MinPrice);
            item.Price.MaxPrice.Should().Be(entity.Price.MaxPrice);
            
            foreach (var option in entity.Options)
            {
                var optionEntity = entity.Options.Single(x => x.OptionNumber.Equals(option.OptionNumber));
            
                optionEntity.OptionNumber.Should().Be(option.OptionNumber);
                optionEntity.Price.Should().Be(option.Price);
            
                optionEntity.SalePricePeriods.Should().BeEquivalentTo(option.SalePricePeriods);
            }

            item.PriceHistory.Should().BeEquivalentTo(entity.PriceHistory);
        }
    }
    
    public void SaleInformationIsNotPresent()
    {
        foreach (var item in _result)
        {
            item.WasPrice.Should().BeNull();
            item.SalePrice.Should().BeNull();
        }
    }

    public void SaleInformationIsPresent()
    {
        foreach (var item in _result)
        {
            item.SalePrice.Should().NotBeNull();
            item.WasPrice.Should().NotBeNull();
            item.PriceHistory.Should().NotBeEmpty();
        }
    }
    
    private static bool IsEquivalentTo(IEnumerable<string> x, IEnumerable<string> ids)
    {
        x.Should().BeEquivalentTo(ids);
        return true;
    }
}