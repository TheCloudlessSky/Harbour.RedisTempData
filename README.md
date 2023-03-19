Harbour.RedisTempData
=====================

This is a [Redis](http://redis.io/) based [ITempDataProvider](http://msdn.microsoft.com/en-us/library/system.web.mvc.itempdataprovider%28v=vs.118%29.aspx) for ASP.NET MVC
written in C# using [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis).

Installation
------------

1. You can either install using NuGet: `PM> Install-Package Harbour.RedisTempData`
2. Or build and install from source: `psake.cmd`

> *Psake Tip:* <br/>Run `psake.cmd -docs` to get a list of the available tasks, such as build, test, public-package, etc.


Publishing Nuget Package
------------------------

In order to publish to Nuget, here are a few simple steps:

1. First you need a nuget package source repository along with an API key and a package source URI for publishing.

2. Now run the `psake publish-package` command from our default.ps1 script, and pass the nuget API key and package source URI from the previous step:
    ```batch
    .\psake.cmd publish-package -properties "@{nugetApiKey='YOUR_NUGET_API_KEY';nugetSource='NUGET_SOURCE_URI'}"
    ```

Usage
-----

MVC has multiple ways to configure the `ITempDataProvider`:

1. Instantiate **per-controller** or from a **base controller**:

    ```csharp
    public abstract class ApplicationController : Controller
    {
        private static readonly ConnectionMultiplexer multiplexer =
            ConnectionMultiplexer.Connect("localhost");
        
        private readonly IDatabase redis = multiplexer.GetDatabase(0);

        protected ApplicationController()
        {
            TempDataProvider = new RedisTempDataProvider(redis);
        }
    }
    
    public class HomeController : ApplicationController
    {
        public ViewResult Index()
        {
            TempData["message"] = "Hello World";
            return View();
        }
    }
    ```

2. In MVC >= 4, use **dependency injection** with your favorite IoC/controller factory. This is the preferred method. For example, with the `Ninject` and `Ninject.MVC3` packages:

    ```csharp
    private static void RegisterServices(IKernel kernel)
    {
        kernel.Bind<ConnectionMultiplexer>()
            .ToMethod(ctx => ConnectionMultiplexer.Connect("localhost"))
            .InSingletonScope();

        kernel.Bind<IDatabase>()
            .ToMethod(ctx => ctx.Kernel.Get<ConnectionMultiplexer>().GetDatabase(0))
            .InRequestScope();

        kernel.Bind<ITempDataProvider>()
            .ToMethod(ctx =>
            {
                var options = new RedisTempDataProviderOptions()
                {
                    KeyPrefix = "MyTempData",
                    KeySeparator = "/",
                    // Serializer = new CustomTempDataSerializer(),
                    // UserProvider = new CustomUserProvider()
                };

                return new RedisTempDataProvider(options, ctx.Kernel.Get<IDatabase>());
            })
            .InRequestScope();
    }        
    ```
    
    **IMPORTANT:**

    Because of [a bug in MVC >= 4](https://aspnetwebstack.codeplex.com/workitem/1692), you must also ensure that you override `CreateTempDataProvider` in your base controller:

    ```csharp
    protected override ITempDataProvider CreateTempDataProvider()
    {
        return DependencyResolver.Current.GetService<ITempDataProvider>();
    }
    ```

Options
-------

You can configure the options such as serialization and how to identify
the current user:

```csharp
var options = new RedisTempDataProviderOptions()
{
    KeyPrefix = "MyTempData",
    KeySeparator = "/",
    Serializer = new CustomTempDataSerializer(),
    UserProvider = new CustomUserProvider()
};

return new RedisTempDataProvider(options, redisClient);
```

Changelog
---------

### v4.0.0

- Targeting only .NET >= 4.6.1. Support for previous .NET versions has been removed.
- Support StackExchange.Redis >= 2.1.30.
- Updated Nuget version from version 2.8.5 to 4.1.0
- Updated psake tooling to support newer .NET versions.

### v3.0.0
- Switch to using [Lua scripts](http://redis.io/commands/eval) instead of MULTI/EXEC.
  However, the underlying data structure has not changed.
- This means you must be running Redis 2.6.0.

### v2.2.1
- Allow overriding how the `DefaultTempDataUserProvider` gets the user from the
  `HttpContextBase`.

### v2.2.0
- Expose the default fallback cookie name: `DefaultTempDataUserProvider.DefaultFallbackCookieName`

### v2.1.2
- Fix performance issue where loading was waiting on the continuation to 
  deserialize. Instead, we want to wait only on getting the result from Redis.
  

### v2.1.1
- Only fall back to the `SessionID` if it's changed since a new `SessionID` is
  generated until the session is used (meaning the TempData would always be
  new).  

### v2.1.0
- Fall back to identifying an anonymous user to the request's `AnonymousID` if
  you're using the `AnonymousIdentificationModule`.

### v2.0.0
- Switch the Redis library to [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis). 
- Change default serializer to `NetDataContractTempDataSerializer`. You can very
  easily implement your own serializer by implementing `ITempDataSerializer`.

### v1.0.1
- Prevent options from being modified once configured for the provider.

### v1.0.0
- Initial release.
