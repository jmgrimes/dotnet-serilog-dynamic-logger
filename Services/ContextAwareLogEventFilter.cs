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
        return ToInt(logEvent.Level) >= ToInt(isParsed ? logEventLevel : _defaultLevel);
    }

    private static int ToInt(LogEventLevel logEventLevel)
    {
        return logEventLevel switch
        {
            LogEventLevel.Verbose => 0,
            LogEventLevel.Debug => 1,
            LogEventLevel.Information => 2,
            LogEventLevel.Warning => 3,
            LogEventLevel.Error => 4,
            LogEventLevel.Fatal => 5,
            _ => -1,
        };
    }
}