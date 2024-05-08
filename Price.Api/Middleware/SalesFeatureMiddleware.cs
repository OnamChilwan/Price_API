using Price.Application.Features;

namespace Price.Api.Middleware;

public class SalesFeatureMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IFeatureFlagRequestContext _featureFlagRequestContext;

    public SalesFeatureMiddleware(
        RequestDelegate next, 
        IFeatureFlagRequestContext featureFlagRequestContext)
    {
        _next = next;
        _featureFlagRequestContext = featureFlagRequestContext;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var featureEnabled = _featureFlagRequestContext.GetValue("price.sale-price.is-enabled");
        context.Features.Set(SalesFeature.Create(featureEnabled));
        await _next(context);
    }
}