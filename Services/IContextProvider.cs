using LaunchDarkly.Sdk;

namespace LaunchDarkly.DynamicLogger.Services;

public interface IContextProvider
{
    Context GetContext();
}