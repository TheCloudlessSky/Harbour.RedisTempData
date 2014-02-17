using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceStack.Text;

namespace Harbour.RedisTempDataSample.Controllers
{
    public class HomeController : ApplicationController
    {
        public class Person
        {
            public int Age { get; set; }
            public string Name { get; set; }

            public Person(int age, string name)
            {
                this.Age = age;
                this.Name = name;
            }

            public Person() { }
        }

        [HttpGet]
        public ActionResult Set()
        {
            TempData["a"] = "Hello World";
            TempData["b"] = 1234;
            TempData["c"] = new Person(24, "John Doe");

            var model = TempData.ToList();
            return Content(model.ToJson());
        }

        [HttpGet]
        public ActionResult Get()
        {
            var model = TempData.Select(kvp => new { Key = kvp.Key, Type = kvp.Value.GetType(), Value = kvp.Value }).ToList();
            return Content(model.ToJson());
        }

        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
