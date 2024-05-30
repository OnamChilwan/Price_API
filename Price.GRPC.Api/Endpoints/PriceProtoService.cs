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
        
        var result = Map(itemPrices);

        foreach (var item in result)
        {
            await responseStream.WriteAsync(item);
        }
    }

    private static IEnumerable<ItemPrice> Map(IEnumerable<ItemPriceDto> dtos)
    {
        foreach (var dto in dtos)
        {
            yield return new ItemPrice
            {
                Territory = dto.Territory,
                ItemNumber = dto.ItemNumber,
                Dataset = dto.Dataset,
                Realm = dto.Realm,
                Id = dto.Id,
                CurrencyCode = dto.CurrencyCode,
                SalePrice = new Pricing
                {
                    MaxPrice = decimal.ToDouble(dto.SalePrice.MaxPrice),
                    MinPrice = decimal.ToDouble(dto.SalePrice.MinPrice)
                },
                WasPrice = new Pricing
                {
                    MaxPrice = decimal.ToDouble(dto.WasPrice.MaxPrice),
                    MinPrice = decimal.ToDouble(dto.WasPrice.MinPrice)
                },
                Price = new Pricing
                {
                    MaxPrice = decimal.ToDouble(dto.Price.MaxPrice),
                    MinPrice = decimal.ToDouble(dto.Price.MinPrice),
                },
                Options =
                {
                    dto.Options.Select(x => new Option
                    {
                        OptionNumber = x.OptionNumber,
                        Price = decimal.ToDouble(x.Price),
                        IsSalePrice = x.IsSalePrice
                    })
                },
                PriceHistory =
                {
                    dto.PriceHistory.Select(x => new History
                    {
                        MaxPrice = decimal.ToDouble(x.MaxPrice ?? 0),
                        MinPrice = decimal.ToDouble(x.MinPrice ?? 0),
                        DatePoint = Convert(x.DatePoint)
                    })
                }
            };

            Timestamp Convert(DateTime timestamp)
            {
                DateTimeOffset offset = timestamp;
                return offset.ToTimestamp();
            }
        }
    }
}