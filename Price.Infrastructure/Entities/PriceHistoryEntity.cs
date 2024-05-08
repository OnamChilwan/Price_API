namespace Price.Infrastructure.Entities;

public class PriceHistoryEntity
{
    public DateTime DatePoint { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }
}