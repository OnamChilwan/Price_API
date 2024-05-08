namespace Price.Infrastructure.Entities;

public class ItemPriceEntity
{
    public string Id { get; set; }

    public string ItemNumber { get; set; }

    public string PriceGroupCode { get; set; }

    public string Realm { get; set; }

    public string Territory { get; set; }

    public string Dataset { get; set; }

    public PriceEntity Price { get; set; }

    public List<OptionEntity> Options { get; set; }

    public List<PriceHistoryEntity>? PriceHistory { get; set; }
}