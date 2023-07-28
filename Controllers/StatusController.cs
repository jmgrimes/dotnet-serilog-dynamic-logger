using LaunchDarkly.DynamicLogger.Resources;
using LaunchDarkly.DynamicLogger.Services;
using LaunchDarkly.Sdk.Server.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LaunchDarkly.DynamicLogger.Controllers;

[ApiController]
[Route("/api/v1/launchdarkly/[controller]")]
public class StatusController: Controller
{
    private readonly ILdClient _ldClient;
    private readonly IConfigurationAccessor _configurationAccessor;
    private readonly ILogger<StatusController> _logger;

    public StatusController(ILdClient ldClient, IConfigurationAccessor configurationAccessor, ILogger<StatusController> logger)
    {
        _ldClient = ldClient;
        _configurationAccessor = configurationAccessor;
        _logger = logger;
    }

    [HttpGet]
    public Status GetStatus()
    {
        _logger.Log(LogLevel.Debug, "processing status request");
        return new Status(
            _ldClient.Initialized, 
            _configurationAccessor.ClientSideId != null,
            _configurationAccessor.MobileKey != null);
    }
}