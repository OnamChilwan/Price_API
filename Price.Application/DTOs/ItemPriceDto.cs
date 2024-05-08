namespace Price.Application.DTOs;

public class ItemPriceDto
{
    public ItemPriceDto(string itemNumber)
        : this(string.Empty, itemNumber, string.Empty, string.Empty, string.Empty, string.Empty, null, new List<OptionDto>(), new List<PriceHistoryDto>())
    {
        ItemNumber = itemNumber;
    }

    public ItemPriceDto(
        string id,
        string itemNumber,
        string realm,
        string territory,
        string dataset,
        string currency,
        PriceDto? price,
        IEnumerable<OptionDto> options,
        IEnumerable<PriceHistoryDto> priceHistory)
    {
        Id = id;
        ItemNumber = itemNumber;
        Realm = realm;
        Territory = territory;
        Dataset = dataset;
        CurrencyCode = currency;
        Price = price;
        Options = options;
        PriceHistory = priceHistory;
    }
    
    public string? Id { get; set; }
    
    public string ItemNumber { get; init; }
    
    public string? Realm { get; set; }
    
    public string? Territory { get; set; }
    
    public string? Dataset { get; set; }
    
    public string? CurrencyCode { get; set; }
    
    public PriceDto? Price { get; set; }
    
    public PriceDto? SalePrice { get; set; }

    public PriceDto? WasPrice { get; set; }
    
    public IEnumerable<OptionDto> Options { get; set; }
    
    public IEnumerable<PriceHistoryDto> PriceHistory { get; set; }
}