using LaunchDarkly.Sdk;

namespace LaunchDarkly.DynamicLogger.Resources;

public class MobileConfiguration
{
    public string MobileKey { get; set; }
    public Context Context { get; set; }

    public MobileConfiguration(string mobileKey, Context context)
    {
        MobileKey = mobileKey;
        Context = context;
    }
}