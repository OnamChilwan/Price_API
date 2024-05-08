using Price.Application.Decorators;
using Price.Application.DTOs;
using Price.Application.Models;
using Price.Infrastructure.Queries;

namespace Price.Application.Services;

public class PriceApplicationService
{
    private readonly IGetMultiplePricesQuery _getMultiplePricesQuery;
    private readonly IDecorator _decorator;

    public PriceApplicationService(
        IGetMultiplePricesQuery getMultiplePricesQuery,
        IDecorator decorator)
    {
        _getMultiplePricesQuery = getMultiplePricesQuery;
        _decorator = decorator;
    }

    public async Task<IEnumerable<ItemPriceDto>> GetMultiplePrices(
        string realm, 
        string territory, 
        string dataset, 
        IEnumerable<string> itemNumbers)
    {
        var ids = itemNumbers
            .Select(x => Id.Create(x, realm, territory, dataset)
                .ToString());
        var itemPriceEntities = await _getMultiplePricesQuery.Execute(ids, realm);
        var context = DecoratorContext.Initialise(itemPriceEntities, itemNumbers);
        var result = await _decorator.Decorate(context);
        
        return result;
    }
}