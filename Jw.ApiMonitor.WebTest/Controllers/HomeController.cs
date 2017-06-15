using Jw.ApiMonitor.Core.MvcFilter;
using System;
using System.Web.Mvc;

namespace Jw.ApiMonitor.WebTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            throw new Exception("出错了");
        }

        [HttpPut]
        public ActionResult Put(string id,int age)
        {
            throw new Exception("出错了");
        }
        public ActionResult GetTest(string id)
        {
            ViewBag.Message = "Your application description page.";

            throw new Exception("出错了");
        }
    }
}