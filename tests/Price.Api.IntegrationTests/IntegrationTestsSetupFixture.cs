using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Price.Api.Configuration;

namespace Price.Api.IntegrationTests;

[SetUpFixture]
public class IntegrationTestsSetupFixture
{
    public CosmosClient CosmosClient { get; private set; }
    
    public CosmosDbSettings CosmosDbSettings { get; private set; }

    [OneTimeSetUp]
    public void Setup()
    {
        new WebHostBuilder()
            .ConfigureServices(sp =>
            {
            })
            .ConfigureAppConfiguration(config =>
            {
                config.AddEnvironmentVariables();
                
                var root = config.Build();
                CosmosDbSettings = new CosmosDbSettings();
                root.Bind("CosmosDb", CosmosDbSettings);
                CosmosClient = new CosmosClient(CosmosDbSettings.Endpoint, CosmosDbSettings.Key);
            })
            .UseStartup<TestStartup>()
            .Build(); 
    }
}