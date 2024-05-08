namespace Price.Api.Models.Responses;

public class ItemPrice
{
    public string Id { get; set; }

    public string ItemNumber { get; set; }

    public string Realm { get; set; }

    public string Territory { get; set; }

    public string Dataset { get; set; }

    public string CurrencyCode { get; set; }

    public Price Price { get; set; }

    public Price SalePrice { get; set; }

    public Price WasPrice { get; set; }

    public IEnumerable<Option> Options { get; set; }

    public IEnumerable<PriceHistory> PriceHistory { get; set; }
}