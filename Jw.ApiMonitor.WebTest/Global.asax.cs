using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Jw.ApiMonitor.Core;
using Jw.ApiMonitor.Core.MvcFilter;
using Jw.ApiMonitor.Core.WebApiFilter;

namespace Jw.ApiMonitor.WebTest
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var apiMonitor = new ApiMonitorAttribute("Jw.ApiMonitor.WebTest");
            apiMonitor.Process = MonitorLogHandler;
            GlobalConfiguration.Configuration.Filters.Add(apiMonitor);
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public void MonitorLogHandler(MonitorInfo log)
        {
            NLog.LogManager.GetLogger("DefaultLog").Info(log.GetLogStr());
        }
    }
}
