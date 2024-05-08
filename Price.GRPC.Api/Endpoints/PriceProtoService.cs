using AutoMapper;
using FluentValidation;
using Grpc.Core;
using Price.Application.DTOs;
using Price.Application.Services;

namespace Price.GRPC.Api.Endpoints;

public class PriceProtoService(
    IMapper mapper,
    IValidator<GetMultipleItemPriceRequest> validator,
    PriceApplicationService priceApplicationService) : PriceService.PriceServiceBase
{
    public override async Task Get(GetMultipleItemPriceRequest request, IServerStreamWriter<ItemPrice> responseStream, ServerCallContext context)
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

        var result = mapper.Map<IEnumerable<ItemPriceDto>, IEnumerable<ItemPrice>>(itemPrices);
        
        foreach (var item in result)
        {
            await responseStream.WriteAsync(item);
        }
    }
}