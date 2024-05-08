namespace Price.Application.Features;

public class DummyFeatureFlagRequestContext : IFeatureFlagRequestContext
{
    public bool GetValue(string flagName)
    {
        return true;
    }
}