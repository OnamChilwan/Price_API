namespace Price.Application.DTOs;

public record PriceHistoryDto(DateTime DatePoint, decimal? MinPrice, decimal? MaxPrice)
{
    // public DateTime DatePoint { get; set; }

    // public decimal? MinPrice { get; set; }

    // public decimal? MaxPrice { get; set; }
}