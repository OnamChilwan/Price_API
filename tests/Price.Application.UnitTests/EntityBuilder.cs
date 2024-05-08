using Bogus;
using Price.Infrastructure.Entities;

namespace Price.Application.UnitTests;

internal class EntityBuilder
{
    private readonly List<ItemPriceEntity> _itemPriceEntities;

    public EntityBuilder()
    {
        _itemPriceEntities = new List<ItemPriceEntity>();
    }

    public EntityBuilder WithItems(params string[] itemNumbers)
    {
        var datasets = new[] { "gold", "bronze", "silver" };
        var realms = new[] { "next", "reiss", "childsplay" };
        var territories = new[] { "GB", "AU", "DE" };
        var priceGroupCodes = new[] { "UK", "GB", "S1" };

        var priceFaker = new Faker<PriceEntity>()
            .RuleFor(x => x.MaxPrice, f => f.Random.Decimal(16, decimal.MaxValue))
            .RuleFor(x => x.MinPrice, f => f.Random.Decimal(5, 15));
        
        var itemPriceFaker = new Faker<ItemPriceEntity>()
            .RuleFor(x => x.Dataset, y => y.PickRandom(datasets))
            .RuleFor(x => x.Id, y => Guid.NewGuid().ToString())
            .RuleFor(x => x.Realm, y => y.PickRandom(realms))
            .RuleFor(x => x.Territory, y => y.PickRandom(territories))
            .RuleFor(x => x.PriceGroupCode, y => y.PickRandom(priceGroupCodes))
            .RuleFor(x => x.Price, priceFaker.Generate());
        
        foreach (var itemNumber in itemNumbers)
        {
            var entity = itemPriceFaker.Generate();
            entity.ItemNumber = itemNumber;
            _itemPriceEntities.Add(entity);
        }
        
        return this;
    }
    
    public EntityBuilder WithOptions(List<OptionEntity>? options = null)
    {
        var optionNumbers = new[] { "01", "02", "03" };
        var optionFaker = new Faker<OptionEntity>()
            .RuleFor(x => x.Price, f => f.Random.Decimal(5, decimal.MaxValue))
            .RuleFor(x => x.OptionNumber, f => f.PickRandom(optionNumbers));

        options ??= optionFaker.Generate(3);
        
        foreach (var entity in _itemPriceEntities)
        {
            entity.Options = options;
        }

        return this;
    }

    public EntityBuilder WithNoActiveSalePeriods(DateTime dateTime)
    {
        var fromDate = dateTime.AddYears(-1);
        var salePricePeriodFaker = new Faker<SalePricePeriodEntity>()
            .RuleFor(x => x.SalePrice, f => f.Random.Decimal(5, decimal.MaxValue))
            .RuleFor(x => x.PriceActiveFrom, f => fromDate)
            .RuleFor(x => x.PriceActiveTo, f => f.Date.Between(fromDate, dateTime.AddSeconds(-1)));
        
        foreach (var entity in _itemPriceEntities)
        {
            foreach (var option in entity.Options)
            {
                option.SalePricePeriods = salePricePeriodFaker.Generate(3);
            }
        }

        return this;
    }
    
    public EntityBuilder WithSingleActiveSalePeriod(out decimal minSalePrice, out decimal maxSalePrice)
    {
        var salePricePeriodFaker = new Faker<SalePricePeriodEntity>()
            .RuleFor(x => x.SalePrice, f => decimal.Round(f.Random.Decimal(5, 1000), 2))
            .RuleFor(x => x.PriceActiveFrom, f => f.Date.Past())
            .RuleFor(x => x.PriceActiveTo, f => f.Date.Future());

        var salePricePeriodEntities = salePricePeriodFaker.Generate(3);
        
        minSalePrice = salePricePeriodEntities.Min(x => x.SalePrice)!.Value;
        maxSalePrice = salePricePeriodEntities.Max(x => x.SalePrice)!.Value;
        
        _itemPriceEntities.First()
            .Options.First()
            .SalePricePeriods = salePricePeriodEntities;

        return this;
    }
    
    public EntityBuilder WithMultipleActiveSalePeriods(out decimal minSalePrice, out decimal maxSalePrice)
    {
        var salePricePeriodFaker = new Faker<SalePricePeriodEntity>()
            .RuleFor(x => x.SalePrice, f => decimal.Round(f.Random.Decimal(5, 1000), 2))
            .RuleFor(x => x.PriceActiveFrom, f => f.Date.Past())
            .RuleFor(x => x.PriceActiveTo, f => f.Date.Future());
        
        _itemPriceEntities
            .ForEach(x => 
                x.Options
                    .ForEach(y => 
                        y.SalePricePeriods = new List<SalePricePeriodEntity>(salePricePeriodFaker.Generate(3))));

        _itemPriceEntities.First()
            .Options.First()
            .SalePricePeriods = new List<SalePricePeriodEntity>();

        var salePricePeriodEntities = new List<SalePricePeriodEntity>();
        
        foreach (var entity in _itemPriceEntities)
        {
            salePricePeriodEntities.AddRange(entity.Options.SelectMany(x => x.SalePricePeriods));
        }

        minSalePrice = salePricePeriodEntities.Min(x => x.SalePrice)!.Value;
        maxSalePrice = salePricePeriodEntities.Max(x => x.SalePrice)!.Value;

        return this;
    }

    public EntityBuilder WithPriceHistory(List<PriceHistoryEntity>? priceHistory = null)
    {
        var historyFaker = new Faker<PriceHistoryEntity>()
            .RuleFor(x => x.DatePoint, f => f.Date.Past())
            .RuleFor(x => x.MaxPrice, f => f.Random.Decimal(11, decimal.MaxValue))
            .RuleFor(x => x.MinPrice, f => f.Random.Decimal(5, 10));

        priceHistory ??= historyFaker.Generate(2);
            
        foreach (var entity in _itemPriceEntities)
        {
            entity.PriceHistory = priceHistory;
        }
        
        return this;
    }
    
    public IEnumerable<ItemPriceEntity> Build()
    {
        return _itemPriceEntities;
    }
}