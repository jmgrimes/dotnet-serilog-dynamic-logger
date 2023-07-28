using LaunchDarkly.Sdk;

namespace LaunchDarkly.DynamicLogger.Resources;

public class ClientConfiguration
{
    public string ClientSideId { get; set; }
    public Context Context { get; set; }

    public ClientConfiguration(string clientSideId, Context context)
    {
        ClientSideId = clientSideId;
        Context = context;
    }
}