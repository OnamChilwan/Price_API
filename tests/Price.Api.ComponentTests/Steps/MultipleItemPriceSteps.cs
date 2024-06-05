using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Google.Protobuf.Collections;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Price.Api.Middleware;
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

    public async Task GrpcRequestIsSent(string realm, string territory, string language, params ItemPriceEntity[] entities)
    {
        var options = new GrpcChannelOptions { HttpHandler = _server.CreateHandler() };
        var channel = GrpcChannel.ForAddress(_server.BaseAddress, options);
        var client = new PriceService.PriceServiceClient(channel);
        var stream = client
            .Get(new GetMultipleItemPriceRequest
            {
                Realm = realm, 
                Territory = territory, 
                Language = language, 
                ItemNumber = { entities.Select(x => x.ItemNumber) }
            })
            .ResponseStream;
        var result = new List<ItemPrice>();

        while (await stream.MoveNext(CancellationToken.None))
        {
            result.Add(stream.Current);
        }

        _result = new List<ItemPrice>(result);
    }

    public async Task AnOKHttpReponseIsReturned()
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
            // item.CurrencyCode.Should().Be(entity) // TODO: config setting

            // item.Price.MinPrice.Should().Be(entity.Price.MinPrice);
            // item.Price.MaxPrice.Should().Be(entity.Price.MaxPrice);
            
            foreach (var option in entity.Options)
            {
                var optionEntity = entity.Options.Single(x => x.OptionNumber.Equals(option.OptionNumber));
            
                optionEntity.OptionNumber.Should().Be(option.OptionNumber);
                optionEntity.Price.Should().Be(option.Price);
                optionEntity.SalePricePeriods.Should().BeEquivalentTo(option.SalePricePeriods);
            }
        }
    }

    public void SaleInformationIsNotPresent(params string[] itemNumbers)
    {
        foreach (var item in _result.Where(x => itemNumbers.Contains(x.ItemNumber)))
        {
            item.SalePrice.Should().BeNull();
            item.PriceHistory.Should().BeEmpty();
        }
    }
    
    public void SaleInformationIsNotPresentForAllItems()
    {
        foreach (var item in _result)
        {
            item.SalePrice.Should().BeNull();
        }
    }
    
    public void SaleInformationIsPresent(ItemPriceEntity entity, decimal salePrice, decimal wasPriceMin, decimal wasPriceMax)
    {
        var item = _result.SingleOrDefault(x => x.ItemNumber.Equals(entity.ItemNumber));

        if (item != null)
        {
            // item.SalePrice.MinPrice.Should().Be(salePrice);
            // item.SalePrice.MaxPrice.Should().Be(salePrice);
            // item.WasPrice.MinPrice.Should().Be(wasPriceMin);
            // item.WasPrice.MaxPrice.Should().Be(wasPriceMax);
        }
    }
    
    public void WasPriceIsPresent(ItemPriceEntity entity, decimal wasPriceMin, decimal wasPriceMax)
    {
       var item = _result.Single(x => x.ItemNumber.Equals(entity.ItemNumber));
       // item.WasPrice.MinPrice.Should().Be(wasPriceMin);
       // item.WasPrice.MaxPrice.Should().Be(wasPriceMax);
       item.PriceHistory.Should().BeEquivalentTo(entity.PriceHistory);
    }

    public void WasPriceIsNotPresent(params string[] itemNumbers)
    {
        var items = _result.Where(x => itemNumbers.Contains(x.ItemNumber));

        foreach (var item in items)
        {
            item.WasPrice.Should().BeNull();
        }
    }

    public void WasPriceIsNotPresentForAllItems()
    {
        foreach (var itemPrice in _result)
        {
            itemPrice.WasPrice.Should().BeNull();
        }
    }
    
    private static bool IsEquivalentTo(IEnumerable<string> x, IEnumerable<string> ids)
    {
        x.Should().BeEquivalentTo(ids);
        return true;
    }
}