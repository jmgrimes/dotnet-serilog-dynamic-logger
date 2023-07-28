using LaunchDarkly.Sdk.Server.Interfaces;
using Serilog.Core;
using Serilog.Events;

namespace LaunchDarkly.DynamicLogger.Services;

public class ContextAwareLogEventFilter: ILogEventFilter
{
    private readonly string _flagName;
    private readonly LogEventLevel _defaultLevel;
    private readonly ILdClient _ldClient;
    private readonly IContextProvider _contextProvider;
    
    public ContextAwareLogEventFilter(
        string flagName, LogEventLevel defaultLevel,
        ILdClient ldClient, IContextProvider contextProvider)
    {
        _flagName = flagName;
        _defaultLevel = defaultLevel;
        _ldClient = ldClient;
        _contextProvider = contextProvider;
    }
    
    
    public bool IsEnabled(LogEvent logEvent)
    {
        var ldContext = _contextProvider.GetContext();
        var logLevelString = _ldClient.StringVariation(_flagName, ldContext, _defaultLevel.ToString())!;
        var isParsed = Enum.TryParse(logLevelString, out LogEventLevel logEventLevel);
        return logEvent.Level.Equals(isParsed ? logEventLevel : _defaultLevel);
    }
}