namespace Price.Infrastructure.Entities;

public class SalePricePeriodEntity
{
    public decimal? SalePrice { get; set; }

    public DateTime? PriceActiveFrom { get; set; }

    public DateTime? PriceActiveTo { get; set; }
}