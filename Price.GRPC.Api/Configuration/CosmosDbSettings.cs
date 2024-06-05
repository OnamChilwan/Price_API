namespace Price.GRPC.Api.Configuration;

public class CosmosDbSettings
{
    public string Endpoint { get; set; } = null!;
    
    public string Key { get; set; } = null!;
    
    public string DatabaseId { get; set; } = null!;
}