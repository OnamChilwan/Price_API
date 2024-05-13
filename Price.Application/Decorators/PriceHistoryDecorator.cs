using Price.Application.DTOs;
using Price.Application.Features;

namespace Price.Application.Decorators;

public class PriceHistoryDecorator : IDecorator
{
    private readonly IDecorator _decorator;
    private readonly SalesFeature _salesFeature;

    public PriceHistoryDecorator(
        IDecorator decorator,
        SalesFeature salesFeature)
    {
        _decorator = decorator;
        _salesFeature = salesFeature;
    }
    
    public async Task<IEnumerable<ItemPriceDto>> Decorate(DecoratorContext context)
    {
        var itemPrices = await _decorator.Decorate(context);
        
        if (_salesFeature.Enabled)
        {
            foreach (var itemPrice in itemPrices)
            {
                var oldestPrice = itemPrice.PriceHistory.MinBy(x => x.DatePoint);

                if (oldestPrice is { MaxPrice: { }, MinPrice: { } })
                {
                    itemPrice.WasPrice = new PriceDto(oldestPrice.MinPrice.Value, oldestPrice.MaxPrice.Value);
                }
            }
        }

        return itemPrices;
    }
}