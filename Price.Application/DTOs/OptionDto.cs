namespace Price.Application.DTOs;

public record OptionDto(string OptionNumber, decimal Price, bool IsSalePrice)
{
}