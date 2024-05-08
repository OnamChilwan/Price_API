namespace Price.Infrastructure.Entities;

public class OptionEntity
{
    public string OptionNumber { get; set; }

    public decimal Price { get; set; }

    public List<SalePricePeriodEntity> SalePricePeriods { get; set; }
}