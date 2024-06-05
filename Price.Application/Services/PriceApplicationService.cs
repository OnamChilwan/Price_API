using Microsoft.Extensions.Logging;
using Price.Application.Decorators;
using Price.Application.DTOs;
using Price.Application.Models;
using Price.Infrastructure.Queries;

namespace Price.Application.Services;

public class PriceApplicationService
{
    private readonly IGetMultiplePricesQuery _getMultiplePricesQuery;
    private readonly IDecorator _decorator;
    private readonly ILogger<PriceApplicationService> _logger;

    public PriceApplicationService(
        IGetMultiplePricesQuery getMultiplePricesQuery,
        IDecorator decorator,
        ILogger<PriceApplicationService> logger)
    {
        _getMultiplePricesQuery = getMultiplePricesQuery;
        _decorator = decorator;
        _logger = logger;
    }

    public async Task<IEnumerable<ItemPriceDto>> GetMultiplePrices(
        string realm, 
        string territory,
        string currency,
        string dataset, 
        IEnumerable<string> itemNumbers)
    {
        var ids = itemNumbers
            .Select(x => Id.Create(x, realm, territory, dataset)
                .ToString());
        
        _logger.LogInformation("Request for IDs {@IDs}, Realm {Realm}, Territory {Territory}, Currency {Currency} and DataSet {DataSet}", 
            ids, realm, territory, currency, dataset);
        
        var itemPriceEntities = await _getMultiplePricesQuery.Execute(ids, realm);
        var context = DecoratorContext.Initialise(itemPriceEntities, itemNumbers, currency, dataset);
        var result = await _decorator.Decorate(context);
        
        return result;
    }
}