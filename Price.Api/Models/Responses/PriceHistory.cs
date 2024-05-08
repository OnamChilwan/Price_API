namespace Price.Api.Models.Responses;

public class PriceHistory
{
    public DateTime DatePoint { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }
}