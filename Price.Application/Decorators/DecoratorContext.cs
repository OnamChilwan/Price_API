using Price.Infrastructure.Entities;

namespace Price.Application.Decorators;

public class DecoratorContext
{
    private DecoratorContext(
        IEnumerable<ItemPriceEntity> entities,
        IEnumerable<string> requestedItemNumbers,
        string currency,
        string dataset)
    {
        Entities = entities.ToList().AsReadOnly();
        RequestedItemNumbers = requestedItemNumbers.ToList().AsReadOnly();
        Currency = currency;
        Dataset = dataset;
    }
    
    public static DecoratorContext Initialise( // TODO: Create a type that encapsulates these so additive changes are easier?
        IEnumerable<ItemPriceEntity> entities,
        IEnumerable<string> requestedItemNumbers,
        string currency,
        string dataset)
    {
        return new DecoratorContext(entities, requestedItemNumbers, currency, dataset);
    }
    
    public IReadOnlyCollection<ItemPriceEntity> Entities { get; }

    public IReadOnlyCollection<string> RequestedItemNumbers { get; }
    
    public string Currency { get; }
    
    public string Dataset { get; }
}