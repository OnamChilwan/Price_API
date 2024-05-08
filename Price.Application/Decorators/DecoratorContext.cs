using Price.Infrastructure.Entities;

namespace Price.Application.Decorators;

public class DecoratorContext
{
    private DecoratorContext(
        IEnumerable<ItemPriceEntity> entities,
        IEnumerable<string> requestedItemNumbers)
    {
        Entities = entities;
        RequestedItemNumbers = requestedItemNumbers;
    }
    
    public static DecoratorContext Initialise(
        IEnumerable<ItemPriceEntity> entities,
        IEnumerable<string> requestedItemNumbers)
    {
        return new DecoratorContext(entities, requestedItemNumbers);
    }
    
    public IEnumerable<ItemPriceEntity> Entities { get; }

    public IEnumerable<string> RequestedItemNumbers { get; }
}