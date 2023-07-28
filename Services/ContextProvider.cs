using LaunchDarkly.Sdk;
using UAParser;

namespace LaunchDarkly.DynamicLogger.Services;

public class ContextProvider: IContextProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ContextProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Context GetContext()
    {
        // TODO Add any other contexts that you might need to share between client and server here.
        var context = Context.MultiBuilder();
        context.Add(GetUserContext());
        context.Add(GetDeviceContext());
        return context.Build();
    }
    
    private Context GetUserContext()
    {
        // TODO Add custom configuration for the user context here.

        // Likely, this will leverage the _httpContextAccessor.HttpContext.User object and some additional database 
        // logic to retrieve information about your user to be returned.  Be sure that you do not include any
        // sensitive information in this context.
        var userKey = Guid.NewGuid().ToString();
        return Context.Builder(ContextKind.Of("user"), userKey)
            .Anonymous(true)
            .Build();
    }

    private Context GetDeviceContext()
    {
        if (_httpContextAccessor.HttpContext != null)
        {
            // Device information provided through UAParser library, or you can add your own for device detection.
            var clientInfo = Parser.GetDefault().Parse(_httpContextAccessor.HttpContext.Request.Headers.UserAgent);
            return Context.Builder(ContextKind.Of("device"), clientInfo.ToString())
                .Set("family", clientInfo.Device.Family)
                .Set("osFamily", clientInfo.OS.Family)
                .Set("osVersion", $"{clientInfo.OS.Major}.{clientInfo.OS.Minor}.{clientInfo.OS.Patch}")
                .Set("uaFamily", clientInfo.UA.Family)
                .Set("uaVersion", $"{clientInfo.UA.Major}.{clientInfo.UA.Minor}.{clientInfo.UA.Patch}")
                .Build();
        }
        var deviceKey = Guid.NewGuid().ToString();
        return Context.Builder(ContextKind.Of("device"), deviceKey)
            .Anonymous(true)
            .Build();
    }
}