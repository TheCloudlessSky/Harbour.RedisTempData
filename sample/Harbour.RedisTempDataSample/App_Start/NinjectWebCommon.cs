[assembly: WebActivator.PreApplicationStartMethod(typeof(Harbour.RedisTempDataSample.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(Harbour.RedisTempDataSample.App_Start.NinjectWebCommon), "Stop")]

namespace Harbour.RedisTempDataSample.App_Start
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using Harbour.RedisTempData;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using StackExchange.Redis;
    
    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            
            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<ConnectionMultiplexer>()
                .ToMethod(ctx => ConnectionMultiplexer.Connect("localhost"))
                .InSingletonScope();

            kernel.Bind<IDatabase>()
                .ToMethod(ctx => ctx.Kernel.Get<ConnectionMultiplexer>().GetDatabase(0))
                .InRequestScope();

            // Besure to override CreateTempDataProvider so that DependencyResolver
            // behaves as expected. See this bug in MVC >= 4:
            // https://aspnetwebstack.codeplex.com/workitem/1692
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
    }
}
