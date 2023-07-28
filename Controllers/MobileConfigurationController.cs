using LaunchDarkly.DynamicLogger.Resources;
using LaunchDarkly.DynamicLogger.Services;
using Microsoft.AspNetCore.Mvc;

namespace LaunchDarkly.DynamicLogger.Controllers;

[ApiController]
[Route("/api/v1/launchdarkly/[controller]")]
public class MobileConfigurationController: ControllerBase
{
    private readonly IConfigurationAccessor _configurationAccessor;
    private readonly IContextProvider _contextProvider;
    private readonly ILogger<MobileConfigurationController> _logger;

    public MobileConfigurationController(
        IConfigurationAccessor configurationAccessor,
        IContextProvider contextProvider,
        ILogger<MobileConfigurationController> logger)
    {
        _configurationAccessor = configurationAccessor;
        _contextProvider = contextProvider;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<MobileConfiguration> GetMobileConfiguration()
    {
        if (_configurationAccessor.MobileKey != null)
        {
            _logger.Log(
                LogLevel.Information,
                "requested mobile configuration; mobile SDK configuration is ENABLED");
            return new MobileConfiguration(_configurationAccessor.MobileKey, _contextProvider.GetContext());
        }
        _logger.Log(
            LogLevel.Warning, 
            "requested mobile configuration; mobile SDK configuration is DISABLED");
        return new NotFoundResult();
    }
}