using Google.Protobuf.WellKnownTypes;
using Price.Application.DTOs;

namespace Price.GRPC.Api.Mappers;

public static class ItemPriceResponseMapper
{
    public static IEnumerable<ItemPrice> Map(IEnumerable<ItemPriceDto> items)
    {
        foreach (var item in items)
        {
            yield return new ItemPrice
            {
                Territory = item.Territory,
                ItemNumber = item.ItemNumber,
                Dataset = item.Dataset,
                Realm = item.Realm,
                Id = item.Id,
                CurrencyCode = item.CurrencyCode,
                SalePrice = MapPrice(item.SalePrice),
                WasPrice = MapPrice(item.WasPrice),
                Price = MapPrice(item.Price),
                Options =
                {
                    item.Options.Select(x => new Option
                    {
                        OptionNumber = x.OptionNumber,
                        Price = decimal.ToDouble(x.Price),
                        IsSalePrice = x.IsSalePrice
                    })
                },
                PriceHistory =
                {
                    item.PriceHistory.Select(x => new History
                    {
                        MaxPrice = decimal.ToDouble(x.MaxPrice ?? 0),
                        MinPrice = decimal.ToDouble(x.MinPrice ?? 0),
                        DatePoint = ConvertToTimeStamp(x.DatePoint)
                    })
                }
            };
        }

        Timestamp ConvertToTimeStamp(DateTime timestamp)
        {
            DateTimeOffset offset = timestamp;
            return offset.ToTimestamp();
        }

        Pricing? MapPrice(PriceDto? price)
        {
            return price == null
                ? null
                : new Pricing
                {
                    MaxPrice = decimal.ToDouble(price.MaxPrice),
                    MinPrice = decimal.ToDouble(price.MinPrice)
                };
        }
    }
}