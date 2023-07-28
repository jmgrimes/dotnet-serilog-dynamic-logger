using LaunchDarkly.DynamicLogger.Resources;
using LaunchDarkly.DynamicLogger.Services;
using Microsoft.AspNetCore.Mvc;

namespace LaunchDarkly.DynamicLogger.Controllers;

[ApiController]
[Route("/api/v1/launchdarkly/[controller]")]
public class ClientConfigurationController: ControllerBase
{
    private readonly IConfigurationAccessor _configurationAccessor;
    private readonly IContextProvider _contextProvider;
    private readonly ILogger<ClientConfigurationController> _logger;
    
    public ClientConfigurationController(
        IConfigurationAccessor configurationAccessor,
        IContextProvider contextProvider,
        ILogger<ClientConfigurationController> logger)
    {
        _configurationAccessor = configurationAccessor;
        _contextProvider = contextProvider;
        _logger = logger;
    }
       
    [HttpGet]
    [Produces("application/json", "application/xml", "application/yaml")]
    public ActionResult<ClientConfiguration> GetClientConfiguration()
    {
        if (_configurationAccessor.ClientSideId != null)
        {
            _logger.Log(
                LogLevel.Information,
                "requested client side configuration; client side SDK configuration is ENABLED");
            return new ClientConfiguration(_configurationAccessor.ClientSideId, _contextProvider.GetContext());
        }
        _logger.Log(
            LogLevel.Warning, 
            "requested client side configuration; client side SDK configuration is DISABLED");
        return new NotFoundResult();
        
    }
}