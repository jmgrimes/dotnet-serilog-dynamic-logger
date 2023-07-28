using LaunchDarkly.Sdk.Server;

namespace LaunchDarkly.DynamicLogger.Services;

public class ConfigurationAccessor: IConfigurationAccessor
{
    public Configuration Configuration { get; }
    
    public string ServerSideKey { get;  }
    public string? ClientSideId { get; }
    public string? MobileKey { get; }

    public ConfigurationAccessor(IConfiguration configuration)
    {
        ServerSideKey = configuration["LaunchDarkly:Sdk:ServerSideKey"]!;
        ClientSideId = configuration["LaunchDarkly:Sdk:ClientSideId"];
        MobileKey = configuration["LaunchDarkly:Sdk:MobileKey"];
        
        // TODO Add any custom configuration you need for the LaunchDarkly client's server configuration here.
        Configuration = Configuration.Default(ServerSideKey);
    }
}