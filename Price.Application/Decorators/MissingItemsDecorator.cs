using Microsoft.Extensions.Logging;
using Price.Application.DTOs;

namespace Price.Application.Decorators;

public class MissingItemsDecorator : IDecorator
{
    private readonly IDecorator _next;
    private readonly ILogger<MissingItemsDecorator> _logger;

    public MissingItemsDecorator(IDecorator next, ILogger<MissingItemsDecorator> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task<IEnumerable<ItemPriceDto>> Decorate(DecoratorContext context)
    {
        var itemPrices = (await _next.Decorate(context)).ToList();

        foreach (var itemNumber in context.RequestedItemNumbers)
        {
            if (itemPrices.Any(dto => dto.ItemNumber.Equals(itemNumber, StringComparison.CurrentCultureIgnoreCase)))
            {
                continue;
            }
            
            _logger.LogInformation("Item {ItemNumber} not found", itemNumber);
            itemPrices.Add(new ItemPriceDto(itemNumber));
        }

        return itemPrices;
    }
}