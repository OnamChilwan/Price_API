using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Price.Application.Features;
using Price.Infrastructure.Queries;

namespace Price.Api.ComponentTests;

public class TestStartup : GRPC.Api.Startup
{
    public TestStartup() : base(BuildConfiguration())
    {
    }

    protected override void ConfigureExternalDependencies(IServiceCollection services)
    {
        services.AddSingleton<IFeatureFlagRequestContext>(_ => Substitute.For<IFeatureFlagRequestContext>());
        services.AddSingleton<IGetMultiplePricesQuery>(_ => Substitute.For<IGetMultiplePricesQuery>());
    
        var context = Substitute.For<IHttpContextAccessor>();
        context.HttpContext.Returns(Substitute.For<HttpContext>());
        
        services.AddSingleton(context);
    }
    
    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();
    }
}