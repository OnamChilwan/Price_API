using System.Collections.Generic;
using System.Linq;
using Price.Api.ComponentTests.Fakes;
using Price.Infrastructure.Entities;

namespace Price.Api.ComponentTests.Builders;

public class ItemPriceBuilder
{
    private readonly List<ItemPriceEntity> _entities = new();

    public ItemPriceBuilder WithItems(params string[] itemNumbers)
    {
        foreach (var itemNumber in itemNumbers)
        {
            var faker = new ItemPriceFaker(itemNumber);
            _entities.Add(faker.Generate());
        }
        
        return this;
    }

    public ItemPriceBuilder WithPriceHistory(params string[] itemNumbers)
    {
        var faker = new PriceHistoryFaker();
        
        foreach (var itemNumber in itemNumbers)
        {
            var item = _entities.SingleOrDefault(x => x.ItemNumber.Equals(itemNumber));
            
            if (item != null)
            {
                item.PriceHistory = faker.Generate(5);    
            }
        }
        
        return this;
    }

    public ItemPriceBuilder WithOptions(string itemNumber, params string[] optionNumbers)
    {
        var item = _entities.SingleOrDefault(x => x.ItemNumber.Equals(itemNumber));
        
        if (item != null)
        {
            item.Options = new List<OptionEntity>();
            
            foreach (var optionNumber in optionNumbers)
            {
                var faker = new OptionFaker(optionNumber);
                item.Options.Add(faker.Generate());
            }
        }
        
        return this;
    }
    
    public ItemPriceBuilder WithActiveSalePeriod(string itemNumber, string optionNumber, out ItemPriceEntity? entity, out decimal price)
    {
        var item = _entities.SingleOrDefault(x => x.ItemNumber.Equals(itemNumber));
        entity = null;
        price = 0M;

        var option = item?.Options.SingleOrDefault(x => x.OptionNumber.Equals(optionNumber));
            
        if (option != null)
        {
            var faker = new SalePricePeriodFaker(true);
            option.SalePricePeriods.Add(faker.Generate());
            entity = item;
            price = option.SalePricePeriods.First().SalePrice.GetValueOrDefault();
        }

        return this;
    }

    public ItemPriceBuilder WithInactiveSalePeriod(string itemNumber, string optionNumber)
    {
        var item = _entities.SingleOrDefault(x => x.ItemNumber.Equals(itemNumber));
        var option = item?.Options.SingleOrDefault(x => x.OptionNumber.Equals(optionNumber));
            
        if (option != null)
        {
            var faker = new SalePricePeriodFaker(false);
            option.SalePricePeriods.Add(faker.Generate());
        }

        return this;
    }

    public IEnumerable<ItemPriceEntity> Build()
    {
        return _entities;
    }
}