using FluentValidation;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Price.Application.Services;
using Price.GRPC.Api.Configuration;
using Price.GRPC.Api.Mappers;

namespace Price.GRPC.Api.Endpoints;

public class PriceProtoService(
    PriceApplicationService priceApplicationService,
    ApiSettings apiSettings,
    IValidator<GetMultipleItemPriceRequest> validator,
    ILogger<PriceProtoService> logger) : PriceService.PriceServiceBase
{
    public override async Task Get(
        GetMultipleItemPriceRequest request,
        IServerStreamWriter<ItemPrice> responseStream,
        ServerCallContext context)
    {
        var dictionary = new Dictionary<string, object>
        {
            { "ItemIds", request.ItemNumber },
            { "Realm", request.Realm },
            { "Territory", request.Territory },
            { "Language", request.Language }
        };

        using (logger.BeginScope(dictionary))
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(',', validationResult.Errors);
                logger.LogInformation("Validation failed due to following errors {@Errors}", errors);
                throw new RpcException(new Status(StatusCode.InvalidArgument, errors), "Invalid Request");
            }

            logger.LogInformation("Validation is successful.");

            var itemPrices = await priceApplicationService.GetMultiplePrices(
                request.Realm,
                request.Territory,
                apiSettings.Currency,
                apiSettings.DataSet,
                request.ItemNumber);

            foreach (var item in ItemPriceResponseMapper.Map(itemPrices))
            {
                await responseStream.WriteAsync(item);
            }
        }
    }
}