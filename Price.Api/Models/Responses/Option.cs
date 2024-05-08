namespace Price.Api.Models.Responses;

public class Option
{
    public string OptionNumber { get; set; }

    public decimal Price { get; set; }

    public bool IsSalePrice { get; set; }
}