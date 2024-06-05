using Microsoft.Extensions.Logging;
using Price.Application.DTOs;
using Price.Application.Features;
using Price.Infrastructure.Entities;

namespace Price.Application.Decorators;

public class SalePriceDecorator : IDecorator
{
    private readonly IDecorator _decorator;
    private readonly SalesFeature _salesFeature;
    private readonly TimeMachineFeature _timeMachineFeature;
    private readonly ILogger<SalePriceDecorator> _logger;

    public SalePriceDecorator(
        IDecorator decorator,
        SalesFeature salesFeature,
        TimeMachineFeature timeMachineFeature,
        ILogger<SalePriceDecorator> logger)
    {
        _decorator = decorator;
        _salesFeature = salesFeature;
        _timeMachineFeature = timeMachineFeature;
        _logger = logger;
    }

    public async Task<IEnumerable<ItemPriceDto>> Decorate(DecoratorContext context)
    {
        var result =  await _decorator.Decorate(context);

        if (!_salesFeature.Enabled)
        {
            _logger.LogInformation("Sales feature is disabled {flag} not applying sale price", _salesFeature.Enabled);
            return result;
        }
        
        foreach (var itemPrice in context.Entities)
        {
            var activeSalePeriods = GetActiveSalePeriodsForOptions(itemPrice.Options);

            if (!activeSalePeriods.Any())
            {
                continue;
            }
            
            var maxPrice = activeSalePeriods.Max(x => x.SalePrice);
            var minPrice = activeSalePeriods.Min(x => x.SalePrice);

            if (maxPrice.HasValue && minPrice.HasValue)
            {
                result
                    .Single(x => x.Id == itemPrice.Id)
                    .SalePrice = new PriceDto(minPrice.Value, maxPrice.Value);
            }
        }

        return result;
    }

    private IEnumerable<SalePricePeriodEntity> GetActiveSalePeriodsForOptions(IEnumerable<OptionEntity> options)
    {
        var activeSalePricePeriods = new List<SalePricePeriodEntity>();
        
        foreach (var option in options)
        {
            if (option.SalePricePeriods is null)
            {
                continue;
            }

            var salePeriods = option.SalePricePeriods
                .Where(sp => _timeMachineFeature.RequestDateTimeUtc >= sp.PriceActiveFrom && _timeMachineFeature.RequestDateTimeUtc < sp.PriceActiveTo)
                .OrderBy(x => x.PriceActiveFrom);
        
            activeSalePricePeriods.AddRange(salePeriods);
        }

        return activeSalePricePeriods;
    }
}