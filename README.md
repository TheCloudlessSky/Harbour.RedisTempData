Harbour.RedisTempData
=====================

This is a [Redis](http://redis.io/) based [ITempDataProvider](http://msdn.microsoft.com/en-us/library/system.web.mvc.itempdataprovider%28v=vs.118%29.aspx) for ASP.NET MVC
written in C# using [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis).

Installation
------------

1. You can either install using NuGet: `PM> Install-Package Harbour.RedisTempData`
2. Or build and install from source: `psake.cmd default`

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
