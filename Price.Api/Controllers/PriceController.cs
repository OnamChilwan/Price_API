using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Price.Api.Models.Requests;
using Price.Api.Models.Responses;
using Price.Application.DTOs;
using Price.Application.Services;

namespace Price.Api.Controllers;

[ApiController]
[Route("price")]
public class PriceController : ControllerBase
{
    private readonly ILogger<PriceController> _logger;
    private readonly IMapper _mapper;
    private readonly PriceApplicationService _priceApplicationService;

    public PriceController(
        ILogger<PriceController> logger, 
        IMapper mapper,
        PriceApplicationService priceApplicationService)
    {
        _logger = logger;
        _mapper = mapper;
        _priceApplicationService = priceApplicationService;
    }

    [HttpGet]
    [Route("{realm}/{territory}/{language}/v1/prices")]
    public async Task<IActionResult> Get(
        [FromRoute] GetMultipleItemPriceRequest request,
        CancellationToken cancellationToken)
    {
        var itemPrices = await _priceApplicationService.GetMultiplePrices(
            request.Realm,
            request.Territory,
            "gbp", // TODO: read from settings
            "gold", // TODO: read from settings
            request.ItemNumber);

        var result = _mapper.Map<IEnumerable<ItemPriceDto>, IEnumerable<ItemPrice>>(itemPrices);
        return new OkObjectResult(result);
    }
}