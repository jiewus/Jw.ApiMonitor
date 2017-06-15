using System;
using System.Net;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace Jw.ApiMonitor.Core.MvcFilter
{
    public class MvcMonitorAttribute : ActionFilterAttribute
    {
        private readonly string Key = "_JwMVCActionMonitorInfo_";

        /// <summary>
        /// 监控信息处理方法
        /// </summary>
        public Action<MonitorInfo> Process { get; set; }

        /// <summary>
        /// 当前服务名称
        /// </summary>
        private string ServiceName { get; set; }

        public MvcMonitorAttribute(string serviceName)
        {
            this.ServiceName = serviceName;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            MonitorInfo monitor = new MonitorInfo()
            {
                ServiceName = this.ServiceName,
                ActionParams = filterContext.ActionParameters,
                ExecuteStartTime = DateTime.Now,
                HttpRequestHeaders = filterContext.HttpContext.Request.Headers.ToString(),
                IpAddr = filterContext.HttpContext.Request.UserHostAddress,
                RequestId = Guid.NewGuid().ToString("N"),
                HttpMethod = filterContext.HttpContext.Request.HttpMethod,
                RequestUrl = filterContext.HttpContext.Request.RawUrl,
            };
            filterContext.Controller.ViewData[Key] = monitor;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            MonitorInfo monitor = filterContext.Controller.ViewData[Key] as MonitorInfo;
            if (monitor == null) return;
            if (string.IsNullOrWhiteSpace(monitor.RequestId)) monitor.RequestId = Guid.NewGuid().ToString("N");
            if (monitor.RequestUrl != filterContext.HttpContext.Request.RawUrl) return;
            monitor.StatusCode = (HttpStatusCode)filterContext.HttpContext.Response.StatusCode;
            monitor.ExecuteEndTime = DateTime.Now;
            if (filterContext.Exception != null)
            {
                monitor.Exception = filterContext.Exception;
            }
            Task.Factory.StartNew(() => { Process(monitor); });
        }
    }
}