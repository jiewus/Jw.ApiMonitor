using System.Web;
using System.Web.Mvc;
using Jw.ApiMonitor.Core;
using Jw.ApiMonitor.Core.MvcFilter;

namespace Jw.ApiMonitor.WebTest
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            var mvcMonitor = new MvcMonitorAttribute("Jw.ApiMonitor.WebTest");
            mvcMonitor.Process = MonitorLogHandler;
            filters.Add(mvcMonitor);
        }

        public static void MonitorLogHandler(MonitorInfo log)
        {
            NLog.LogManager.GetLogger("DefaultLog").Info(log.GetLogStr());
        }
    }
}
