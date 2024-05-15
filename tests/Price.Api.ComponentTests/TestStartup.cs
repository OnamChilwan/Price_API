using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Price.Api.Middleware;
using Price.Application.Features;
using Price.Infrastructure.Queries;

namespace Price.Api.ComponentTests;

public class TestStartup : Startup
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
        
        //context.HttpContext?.Features.Set(SalesFeature.Default());
        //context.HttpContext?.Features.Set(TimeMachineFeature.Default());
    
        services.AddSingleton(context);
    }
    
    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();
    }
}