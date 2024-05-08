using Microsoft.AspNetCore.Mvc;

namespace Price.Api.Models.Requests;

public class GetMultipleItemPriceRequest
{
    [FromRoute]
    public string Realm { get; set; }

    [FromRoute]
    public string Territory { get; set; }

    [FromRoute]
    public string Language { get; set; }
    
    [FromQuery]
    public IEnumerable<string> ItemNumber { get; set; }
}