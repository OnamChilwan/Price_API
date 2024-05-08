using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Price.Infrastructure.Factories;
using Price.Infrastructure.Queries;

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