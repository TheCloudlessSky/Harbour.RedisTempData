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
        public static readonly string PageCountKey = "pageCount";

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

        protected void IncrementPageCount()
        {
            // NOTE: First time logging in, TempData is saved under previous anonymous ID, so we skip
            //       the first page tally or it will be stored under the anonymous ID in Redis.
            bool newLoginVal = false;
            var newLogin = TempData["newLogin"];
            if (newLogin != null)
            {
                newLoginVal = (bool)newLogin;
            }

            if (!newLoginVal)
            {
                var tempPageCount = TempData[PageCountKey];
                int pageCount = (tempPageCount == null) ? 1 : (int)tempPageCount;
                pageCount += 1;
                TempData[PageCountKey] = pageCount;
            }
        }
    }
}
