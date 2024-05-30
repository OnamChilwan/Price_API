using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Price.Application.DTOs;
using Price.Application.Services;

namespace Price.GRPC.Api.Endpoints;

public class PriceProtoService(
    IValidator<GetMultipleItemPriceRequest> validator,
    PriceApplicationService priceApplicationService) : PriceService.PriceServiceBase
{
    public override async Task Get(GetMultipleItemPriceRequest request, IServerStreamWriter<ItemPrice> responseStream,
        ServerCallContext context)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(',', validationResult.Errors);
            throw new RpcException(new Status(StatusCode.InvalidArgument, errors), "Invalid Request");
        }

        var itemPrices = await priceApplicationService.GetMultiplePrices(
            request.Realm,
            request.Territory,
            "gold",
            request.ItemNumber);

        foreach (var item in Map(itemPrices))
        {
            await responseStream.WriteAsync(item);
        }
    }

    private static IEnumerable<ItemPrice> Map(IEnumerable<ItemPriceDto> items)
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
            return price == null ? null : new Pricing
            {
                MaxPrice = decimal.ToDouble(price.MaxPrice),
                MinPrice = decimal.ToDouble(price.MinPrice)
            };
        }
    }
}