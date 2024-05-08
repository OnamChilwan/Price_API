namespace Price.Application.Features;

public interface IFeatureFlagRequestContext
{
    bool GetValue(string flagName);
}