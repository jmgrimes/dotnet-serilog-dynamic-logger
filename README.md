# LaunchDarkly .NET Backend Quickstart

The LaunchDarkly .NET Backend Quickstart is a simple template application developed to help you get started with the 
LaunchDarkly SDK in your server side .NET application quickly.

## Prerequisites

- Access to the LaunchDarkly platform to configure a new project and environment, or access to the SDK keys for an 
- existing project.
- .NET Core Framework 7.0

## Getting Started

To get started, copy this folder as the basis for your new project to a location on your workstation.  Rename the 
project as desired, and create a new repository to house the application components, and add the files to the 
repository.

Once you've set up a repository with these components, you will need to configure the LaunchDarkly SDK and endpoint 
support, depending on your needs.  Out of the box, this template draws configuration from the standard .NET 
configuration mechanisms supported by appsettings.json.  To get moving quickly:

1) Create a new appsettings.Development.json file at the root of your project.  Do not add this file to source control.

2) Create a new LaunchDarkly project for your application (or use an existing one).  Add any environments that you need 
   to this project.  For the development environment, take note of the server side SDK Key, Client Side ID, and Mobile 
   Key.

3) Add the following configuration to the appsettings.Development.json file:
   
   ```json
   {
     "LaunchDarkly": {
         "Sdk": {
             "ClientSideId": "<my-client-side-id>",
             "MobileKey": "<my-mobile-key>",
             "ServerSideKey": "<my-server-side-key>"
         }
     }
   }
   ```

4) Substitute the keys noted from step 2 into the appropriate placeholder configuration values.  If you don't need 
   mobile or frontend support, you can remove the "ClientSideId" and "MobileKey" options instead, disabling the 
   mobile and frontend endpoints.

> We recommend that you store your SDK Keys for this configuration in a dedicated configuration store, such as AWS 
> Secrets Manager, or otherwise inject these values into your configuration at runtime, rather than using files.  If 
> you choose to use files, DO NOT store your SDK Keys in configuration files that are committed to your project 
> repository.

5) Run the application with the command ```dotnet run``` from the project root directory.  The application will start 
   up and listen for web requests on port 5100.

## Customizing the Context

The quickstart provides a default context object out of the box for your application to use.  You will almost certainly 
need to customize this, based on the client requests that your application receives.  This object is provided by the 
```ContextProvider``` class in the Services package.  Override the ```GetContext``` method here as desired to change 
the contexts available, based on the user's session or request.

> One important caveat to customizing the context is that it's important to use only session scoped or generally static 
> details for the context created by ```ContextProvider```.  While it is possible to use more complex request or model 
> scoped contexts with the server side LaunchDarkly SDK, the contexts returned by the ```ContextProvider``` are also 
> used when initializing and re-identifying on the client or mobile side.  As such, these will never be in sync if you
> are using request specific attributes and contexts.

## Customizing the Configuration

The LaunchDarkly SDK is configured using the ```ConfigurationAccessor``` class in the Services package.  You can 
customize the configuration object in the constructor of this class, if you need to modify any of the default options 
or change the way in which SDK Keys are retrieved.

## Using the LaunchDarkly SDK

This template uses .NET services to create and manage an instance of the LaunchDarkly SDK for you, as well as a 
configured instance of the IContextProvider interface to handle retrieval of the LaunchDarkly SDK evaluation context.  
You do not need to create a new ```LDClient``` instance manually.

To use the managed LaunchDarkly SDK client and context provider, you can use the default .NET framework related 
dependency injection or service provider mechanisms.

For example, you can access the LaunchDarkly SDK in an ApiController by using constructor injection of the LaunchDarkly 
SDK client and the context provider as below:

```csharp
[ApiController]
[Route("/api/v1/myapp/[controller]")]
public class MyAppController: ControllerBase
{
    private readonly ILdClient _ldClient;
    private readonly IContextProvider _contextProvider;
    
    public ClientConfigurationController(
        ILdClient ldClient,
        IContextProvider contextProvider)
    {
        _ldClient = ldClient;
        _contextProvider = contextProvider;
    }
       
    [HttpGet]
    public ActionResult GetResult()
    {
        var context = _contextProvider.GetContext()
        if (_ldClient.BoolVariation("my-feature-flag", context, false)) {
            // feature flag is on, do enabled functionality.
        }
        // feature flag is off, do disabled functionality.
    }
}
```