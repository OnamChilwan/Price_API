using Price.Application.DTOs;
using Price.Infrastructure.Entities;

namespace Price.Application.Decorators;

public class MapItemPriceDecorator : IDecorator
{
    public Task<IEnumerable<ItemPriceDto>> Decorate(DecoratorContext context)
    {
        var result = new List<ItemPriceDto>();

        foreach (var entity in context.Entities)
        {
            result.Add(new ItemPriceDto(
                entity.Id,
                entity.ItemNumber,
                entity.Realm,
                entity.Territory,
                entity.Dataset,
                "GBP",
                new PriceDto(entity.Price.MinPrice, entity.Price.MaxPrice),
                MapOptions(entity),
                MapPriceHistory(entity)));
            
            // {
            //     Dataset = entity.Dataset,
            //     Id = entity.Id,
            //     Realm = entity.Realm,
            //     Territory = entity.Territory,
            //     CurrencyCode = "GBP", // Default
            //     ItemNumber = entity.ItemNumber,
            //     Price = new PriceDto(entity.Price.MinPrice, entity.Price.MaxPrice),
            //     Options = MapOptions(entity),
            //     PriceHistory = MapPriceHistory(entity),
            //     SalePrice = null, //new PriceDto(),
            //     WasPrice = null //new PriceDto()
            // });
        }

        return Task.FromResult<IEnumerable<ItemPriceDto>>(result);
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