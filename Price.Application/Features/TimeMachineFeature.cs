namespace Price.Application.Features;

public class TimeMachineFeature
{
    private TimeMachineFeature(DateTime requestDateTimeUtc)
    {
        this.RequestDateTimeUtc = requestDateTimeUtc;
    }
    
    public static TimeMachineFeature Create(DateTime requestDateTimeUtc)
    {
        return new TimeMachineFeature(requestDateTimeUtc);
    }

    public static TimeMachineFeature Default()
    {
        return new TimeMachineFeature(DateTime.UtcNow);
    }
    
    public DateTime RequestDateTimeUtc { get; }
}