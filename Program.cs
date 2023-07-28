using LaunchDarkly.DynamicLogger.Services;
using LaunchDarkly.Sdk.Server;
using LaunchDarkly.Sdk.Server.Interfaces;
using Serilog;
using Serilog.Core;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IConfigurationAccessor, ConfigurationAccessor>();
builder.Services.AddSingleton<IContextProvider, ContextProvider>();
builder.Services.AddSingleton<ILdClient>(sp =>
{
    // WARNING: DO NOT configure LdClient to log through Serilog, or you will create an
    // infinite loop while using the ContextAwareLogEventFilter, since it performs flag
    // evaluations with the LdClient instance created here.
    var configurationAccessor = sp.GetService<IConfigurationAccessor>()!;
    return new LdClient(configurationAccessor.Configuration);
});
builder.Services.AddSingleton<ILogEventFilter>(sp =>
{
    // See above warnings about the LdClient logging setup.  Make sure that LdClient is set up
    // to log through console, or something OTHER than Serilog, or this will create a potential
    // infinite loop.
    var ldClient = sp.GetService<ILdClient>()!;
    
    // The context provider is used to automatically detect and leverage a LaunchDarkly context
    // object based on the current request.  In this way, you don't need to directly pass the 
    // Context into your log statements.  This happens transparently.  You can update the strategy
    // for providing the context based on your own requirements.
    var contextProvider = sp.GetService<IContextProvider>()!;
    
    // The flag name to read log level configuration from.  This flag must return a string of the 
    // same string format of the LogEventLevel enum.
    const string flagName = "configure-log-level";
    
    // If the flag cannot be found, or if the value the flag returns is not parseable to an instance 
    // of LogEventLevel, this default level will be used.  Anything at or higher than this level will 
    // pass through to the Sink.
    const LogEventLevel defaultLevel = LogEventLevel.Debug;
    return new ContextAwareLogEventFilter(flagName, defaultLevel, ldClient, contextProvider);
});

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
