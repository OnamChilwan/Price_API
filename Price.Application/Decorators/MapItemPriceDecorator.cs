using Price.Application.DTOs;
using Price.Infrastructure.Entities;

namespace Price.Application.Decorators;

public class MapItemPriceDecorator : IDecorator
{
    public Task<IEnumerable<ItemPriceDto>> Decorate(DecoratorContext context)
    {
        var result = context
            .Entities
            .Select(entity => new ItemPriceDto(
                entity.Id, 
                entity.ItemNumber, 
                entity.Realm, 
                entity.Territory, 
                entity.Dataset, // TODO: driven by settings
                context.Currency,
                MapPrice(entity.Price.MinPrice, entity.Price.MaxPrice), 
                MapOptions(entity), 
                MapPriceHistory(entity)))
            .ToList();

        return Task.FromResult<IEnumerable<ItemPriceDto>>(result);
    }

    private static PriceDto MapPrice(decimal minPrice, decimal maxPrice)
    {
        return new PriceDto(minPrice, maxPrice);
    }

    private static IEnumerable<PriceHistoryDto> MapPriceHistory(ItemPriceEntity entity)
    {
        if (entity.PriceHistory is null || !entity.PriceHistory.Any())
        {
            return Enumerable.Empty<PriceHistoryDto>();
        }

        return entity.PriceHistory.Select(history => 
            new PriceHistoryDto(history.DatePoint, history.MinPrice, history.MaxPrice));
    }

    private static IEnumerable<OptionDto> MapOptions(ItemPriceEntity entity)
    {
        if (entity.Options is null || !entity.Options.Any())
        {
            return Enumerable.Empty<OptionDto>();
        }
        
        return entity.Options.Select(opt => 
            new OptionDto(opt.OptionNumber, opt.Price, false));
    }
}