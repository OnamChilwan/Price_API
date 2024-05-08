using Price.Application.Features;

namespace Price.Api.Middleware;

public class TimeMachineMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IFeatureFlagRequestContext _featureFlagRequestContext;

    public TimeMachineMiddleware(
        RequestDelegate next, 
        IFeatureFlagRequestContext featureFlagRequestContext)
    {
        _next = next;
        _featureFlagRequestContext = featureFlagRequestContext;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var featureEnabled = _featureFlagRequestContext.GetValue("price.time-machine.is-enabled");
        var requestDateTimeUtc = DateTime.UtcNow;

        if (featureEnabled)
        {
            if (context.Request.Headers.TryGetValue("x-next-time-machine-date", out var value))
            {
                DateTime.TryParse(value.First(), out requestDateTimeUtc);
            }
        }
        
        context.Features.Set(TimeMachineFeature.Create(requestDateTimeUtc));
        await _next(context);
    }
}