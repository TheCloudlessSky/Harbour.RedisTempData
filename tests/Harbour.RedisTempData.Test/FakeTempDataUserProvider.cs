using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Harbour.RedisTempData.Test
{
    public class FakeTempDataUserProvider : ITempDataUserProvider
    {
        private readonly string user;

        public FakeTempDataUserProvider(string user)
        {
            this.user = user;
        }

        public string GetUser(ControllerContext context)
        {
            return user;
        }
    }
}
