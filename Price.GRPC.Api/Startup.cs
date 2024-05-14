using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Price.Application.Decorators;
using Price.Application.DTOs;
using Price.Application.Features;
using Price.Application.Services;
using Price.GRPC.Api.Endpoints;
using Price.GRPC.Api.Middleware;
using Price.GRPC.Api.Validators;
using Price.Infrastructure.Queries;

namespace Price.GRPC.Api;

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
            endpoints.MapGrpcReflectionService();
            endpoints.MapGrpcService<PriceProtoService>();
        });
    }

    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureExternalDependencies(services);
        
        services.AddGrpc(options =>
        {
            // options.Interceptors.Add<ServerLoggerInterceptor>();
        });
        services.AddGrpcReflection();
        
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient<IFeatureFlagRequestContext, DummyFeatureFlagRequestContext>();
        services.AddScoped<IValidator<GetMultipleItemPriceRequest>, GetMultipleItemPriceRequestValidator>();
        
        // Decorators
        services.AddScoped<IDecorator, MapItemPriceDecorator>();
        services.Decorate<IDecorator, PriceHistoryDecorator>();
        services.Decorate<IDecorator, SalePriceDecorator>();
        services.Decorate<IDecorator, MissingItemsDecorator>();
        
        services.AddTransient<PriceApplicationService>();

        ConfigureMapper(services);
        ConfigureFeatures(services);
    }
    
    protected virtual void ConfigureExternalDependencies(IServiceCollection services)
    {
        services.AddTransient<IGetMultiplePricesQuery, FakeGetMultiplePricesQuery>();
    }
    
    private static void ConfigureMapper(IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ItemPrice).Assembly, typeof(PriceDto).Assembly);
    }

    private static void ConfigureFeatures(IServiceCollection services)
    {
        services.AddScoped(sp =>
        {
            var ctx = sp.GetRequiredService<IHttpContextAccessor>();
            var feature = ctx.HttpContext.Features.Get<SalesFeature>();
            return feature ?? SalesFeature.Default();   
        });
    
        services.AddScoped(sp =>
        {
            var ctx = sp.GetRequiredService<IHttpContextAccessor>();
            var feature = ctx.HttpContext.Features.Get<TimeMachineFeature>();
            return feature ?? TimeMachineFeature.Default();
        });
    }
}