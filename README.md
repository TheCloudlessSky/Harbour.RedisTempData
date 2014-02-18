Harbour.RedisTempData
=====================

This is a [Redis](http://redis.io/) based [ITempDataProvider](http://msdn.microsoft.com/en-us/library/system.web.mvc.itempdataprovider%28v=vs.118%29.aspx) for ASP.NET MVC
written in C# using [ServiceStack.Redis](https://github.com/ServiceStack/ServiceStack.Redis).

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
	    // You should use an IRedisClientsManager to resolve the client
	    // such as PooledRedisClientManager or BasicRedisClientManager.
	    // The best approach would be to wire this up with IoC.
	    private readonly IRedisClient redis = new RedisClient("localhost:6379");
	
	    protected ApplicationController()
	    {
	        TempDataProvider = new RedisTempDataProvider(redis);
	    }
	
	    protected override void Dispose(bool disposing)
	    {
	        redis.Dispose();
	
	        base.Dispose(disposing);
	    }
	}
	
	public class HomeController : HomeController
	{
	    public ViewResult Index()
	    {
	        TempData["message"] = "Hello World";
	        return View();
	    }
	}
	```

2. Use **dependency injection** with your favorite IoC/controller factory. This
   is the preferred method. For example, with the `Ninject` and `Ninject.MVC3` packages:

	```csharp
	private static void RegisterServices(IKernel kernel)
    {
        // Ideally you'd use PooledClientManager in production!
        kernel.Bind<IRedisClientsManager>()
            .To<BasicRedisClientManager>()
            .InSingletonScope();

        kernel.Bind<IRedisClient>()
            .ToMethod(ctx => ctx.Kernel.Get<IRedisClientsManager>().GetClient())
            .InRequestScope();

        kernel.Bind<ITempDataProvider>()
            .To<RedisTempDataProvider>()
            .InRequestScope();
    }        
	```

You can also configure the options such as serialization and how to identify
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

### v1.0.1
- Prevent options from being modified once configured for the provider.

### v1.0.0
- Initial release.