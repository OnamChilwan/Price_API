using FluentValidation;
using Microsoft.Azure.Cosmos;
using Price.Api.Configuration;
using Price.Api.Middleware;
using Price.Api.Models.Responses;
using Price.Api.Validators;
using Price.Application.Decorators;
using Price.Application.DTOs;
using Price.Application.Features;
using Price.Application.Services;
using Price.Infrastructure.Factories;
using Price.Infrastructure.Queries;

namespace Price.Api;

public class Startup
{
    private readonly IConfiguration _configuration;
    
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        app.UseMiddleware<SalesFeatureMiddleware>();
        app.UseMiddleware<TimeMachineMiddleware>();
        app.UseRouting();
        app.UseEndpoints(endpoints => 
        {
            endpoints.MapControllers();
        });
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers().AddApplicationPart(typeof(Startup).Assembly);
        services.AddRouting();
        
        services.AddValidatorsFromAssemblyContaining<GetMultipleItemPriceRequestValidator>();
        
        // Decorators
        services.AddScoped<IDecorator, MapItemPriceDecorator>();
        services.Decorate<IDecorator, PriceHistoryDecorator>();
        services.Decorate<IDecorator, SalePriceDecorator>();
        services.Decorate<IDecorator, MissingItemsDecorator>();
        
        services.AddTransient<PriceApplicationService>();

        ConfigureMapper(services);
        ConfigureFeatures(services);
        
        ConfigureExternalDependencies(services);
    }

    protected virtual void ConfigureExternalDependencies(IServiceCollection services)
    {
        services.AddTransient<IFeatureFlagRequestContext, DummyFeatureFlagRequestContext>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        
        var cosmosDbSettings = new CosmosDbSettings();
        _configuration.Bind("CosmosDb", cosmosDbSettings);
        
        var cosmosClient = new CosmosClient(cosmosDbSettings.Endpoint, cosmosDbSettings.Key);
        services.AddTransient(_ => new CosmosContainerFactory(cosmosClient, cosmosDbSettings.DatabaseId));
        services.AddTransient<IGetMultiplePricesQuery, CosmosGetMultiplePricesQuery>();
        services.AddTransient<IGetMultiplePricesQuery, FakeGetMultiplePricesQuery>();
    }

    private static void ConfigureMapper(IServiceCollection services)
    {
        // services.AddAutoMapper(typeof(ItemPrice).Assembly, typeof(PriceDto).Assembly);
    }

    private static void ConfigureFeatures(IServiceCollection services)
    {
        services.AddScoped(sp =>
        {
            var ctx = sp.GetRequiredService<IHttpContextAccessor>();
            var feature = ctx.HttpContext!.Features.Get<SalesFeature>();
            return feature ?? SalesFeature.Default();   
        });

        services.AddScoped(sp =>
        {
            var ctx = sp.GetRequiredService<IHttpContextAccessor>();
            var feature = ctx.HttpContext!.Features.Get<TimeMachineFeature>();
            return feature ?? TimeMachineFeature.Default();
        });
    }
}