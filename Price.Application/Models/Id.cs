namespace Price.Application.Models;

public class Id
{
    private readonly string _itemNumber;
    private readonly string _realm;
    private readonly string _territory;
    private readonly string _dataset;

    private Id(string itemNumber,
        string realm,
        string territory,
        string dataset)
    {
        _itemNumber = itemNumber;
        _realm = realm;
        _territory = territory;
        _dataset = dataset;
    }
    
    public static Id Create(
        string itemNumber,
        string realm,
        string territory,
        string dataset)
    {
        return new Id(itemNumber, realm, territory, dataset);
    }

    public override string ToString()
    {
        return $"{_realm.ToLower()}-{_territory.ToLower()}-{_dataset.ToLower()}-{_itemNumber}";
    }
}