using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Harbour.RedisTempData;
using StackExchange.Redis;

namespace Harbour.RedisTempDataSample.Controllers
{
    public abstract class ApplicationController : Controller
    {
        // Must be overridden because of bug in MVC:
        // https://aspnetwebstack.codeplex.com/workitem/1692
        protected override ITempDataProvider CreateTempDataProvider()
        {
            return DependencyResolver.Current.GetService<ITempDataProvider>();
        }

        // If you're not using an IoC container, you can do this.
        // private static readonly ConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect("localhost");
        // private readonly IDatabase redis = multiplexer.GetDatabase(0);
        // protected override ITempDataProvider CreateTempDataProvider()
        // {
        //     return new RedisTempDataProvider(redis);
        // }
    }
}
