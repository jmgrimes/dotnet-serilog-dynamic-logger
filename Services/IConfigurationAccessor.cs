using LaunchDarkly.Sdk.Server;

namespace LaunchDarkly.DynamicLogger.Services;

public interface IConfigurationAccessor
{
    string ServerSideKey { get; }
    string? ClientSideId { get; }

    string? MobileKey { get; }
    
    Configuration Configuration { get; }
}