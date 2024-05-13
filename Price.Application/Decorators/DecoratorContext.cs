using Price.Infrastructure.Entities;

namespace Price.Application.Decorators;

public class DecoratorContext
{
    private DecoratorContext(
        IEnumerable<ItemPriceEntity> entities,
        IEnumerable<string> requestedItemNumbers)
    {
        Entities = entities.ToList().AsReadOnly();
        RequestedItemNumbers = requestedItemNumbers.ToList().AsReadOnly();
    }
    
    public static DecoratorContext Initialise(
        IEnumerable<ItemPriceEntity> entities,
        IEnumerable<string> requestedItemNumbers)
    {
        return new DecoratorContext(entities, requestedItemNumbers);
    }
    
    public IReadOnlyCollection<ItemPriceEntity> Entities { get; }

    public IReadOnlyCollection<string> RequestedItemNumbers { get; }
}