using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Harbour.RedisTempData;
using ServiceStack.Redis;

namespace Harbour.RedisTempDataSample.Controllers
{
    public abstract class ApplicationController : Controller
    {
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
}
