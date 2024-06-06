using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Client;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Price.Infrastructure.Factories;
using Price.Infrastructure.Queries;
using Wrapr;

namespace Price.Api.IntegrationTests.Queries;

[TestFixture]
public class CosmosGetMultiplePricesQueryTests : IntegrationTestsSetupFixture
{
    [Test]
    public async Task Given_Next_Document_Exists_When_Querying_Document_Is_Returned()
    {
        const string id = "next-gb-gold-400001";
        var subject = new CosmosGetMultiplePricesQuery(new CosmosContainerFactory(CosmosClient, CosmosDbSettings.DatabaseId));
        var result = await subject.Execute(new[] { id }, "next");

        result.Should().NotBeEmpty();
    }
    
    [Test]
    public async Task Given_Non_Next_Document_Exists_In_When_Querying_Document_Is_Returned()
    {
        const string id = "gap-gb-gold-700005";
        var subject = new CosmosGetMultiplePricesQuery(new CosmosContainerFactory(CosmosClient, CosmosDbSettings.DatabaseId));
        var result = await subject.Execute(new[] { id }, "gap");

        result.Should().NotBeEmpty();
    }
}

public class QueryTests
{
    [Test]
    public async Task Dapr_Query_Test()
    {
        var currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory());
        var componentPathInfo = TryGetComponentPath(currentDirectory);
        await using var sidecar = new Sidecar("integration-test");
        
        try
        {
            await sidecar.Start(with => with
                .ResourcesPath(componentPathInfo.FullName)
                .DaprGrpcPort(1111)
                .Args("--log-level", "warn"));
        
            using var client = new DaprClientBuilder()
                .UseGrpcEndpoint("http://localhost:1234")
                .Build();

            var logger = LoggerFactory
                .Create(b => b.AddConsole())
                .CreateLogger<DaprGetMultiplePricesQuery>();
            
            var subject = new DaprGetMultiplePricesQuery(client, logger);

            var result = await subject.Execute(new[] { "" }, "");
        }
        finally
        {
            await sidecar.Stop();    
        }
    }

    private static DirectoryInfo TryGetComponentPath(DirectoryInfo? dir)
    {
        var result = dir
            .GetDirectories()
            .FirstOrDefault(x => x.Name.Equals("components"));
        
        if (result is null)
        {
            return TryGetComponentPath(dir.Parent);
        }

        return result;
    }
}