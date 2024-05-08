using Price.Application.DTOs;

namespace Price.Application.Decorators;

// TODO: Unit tests
public class MissingItemsDecorator : IDecorator
{
    private readonly IDecorator _next;

    public MissingItemsDecorator(IDecorator next)
    {
        _next = next;
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
            
            itemPrices.Add(new ItemPriceDto(itemNumber));
        }

        return itemPrices;
    }
}