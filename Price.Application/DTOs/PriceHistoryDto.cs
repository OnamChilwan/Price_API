namespace Price.Application.DTOs;

public record PriceHistoryDto(DateTime DatePoint, decimal? MinPrice, decimal? MaxPrice);