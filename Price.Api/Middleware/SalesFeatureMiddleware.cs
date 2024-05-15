using Price.Application.Features;

namespace Price.Api.Middleware;

public class SalesFeatureMiddleware
{
    public const string FlagName = "price.sale-price.is-enabled";
    
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
        var featureEnabled = _featureFlagRequestContext.GetValue(FlagName);
        context.Features.Set(SalesFeature.Create(featureEnabled));
        await _next(context);
    }
}