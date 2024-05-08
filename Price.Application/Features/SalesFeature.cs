namespace Price.Application.Features;

public class SalesFeature
{
    private SalesFeature(bool enabled)
    {
        Enabled = enabled;
    }

    public static SalesFeature Create(bool enabled)
    {
        return new SalesFeature(enabled);
    }

    public static SalesFeature Default()
    {
        return new SalesFeature(false);
    }
    
    public bool Enabled { get; }
}