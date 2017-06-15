using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Threading.Tasks;

namespace Jw.ApiMonitor.Core.WebApiFilter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ApiMonitorAttribute : ActionFilterAttribute
    {
        private readonly string Key = "_JwWebApiActionMonitorInfo_";

        /// <summary>
        /// 监控信息处理方法
        /// </summary>
        public Action<MonitorInfo> Process { get; set; }

        /// <summary>
        /// 当前服务名称
        /// </summary>
        private string ServiceName { get; set; }

        public ApiMonitorAttribute(string serviceName)
        {
            this.ServiceName = serviceName;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            MonitorInfo monitor = new MonitorInfo()
            {
                ServiceName = this.ServiceName,
                ActionParams = actionContext.ActionArguments,
                ExecuteStartTime = DateTime.Now,
                HttpMethod = actionContext.Request.Method.Method,
                HttpRequestHeaders = actionContext.Request.Headers.ToString(),
                RequestIpAddr = actionContext.Request.RequestUri.Host,
                RequestUrl = actionContext.Request.RequestUri.LocalPath,
                RequestId = Guid.NewGuid().ToString("N"),
                HttpContext = System.Web.HttpContext.Current
            };
            monitor.ExecuteStartTime = DateTime.Now;
            actionContext.Request.Properties[Key] = monitor;
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            MonitorInfo monitor = actionExecutedContext.Request.Properties[Key] as MonitorInfo;
            if (monitor == null) return;
            if (string.IsNullOrWhiteSpace(monitor.RequestId)) monitor.RequestId = Guid.NewGuid().ToString("N");
            monitor.ExecuteEndTime = DateTime.Now;
            if (monitor.RequestUrl != actionExecutedContext.Request.RequestUri.LocalPath) return;
            monitor.StatusCode = actionExecutedContext.Response.StatusCode;
            if (actionExecutedContext.Exception != null)
            {
                monitor.Exception = actionExecutedContext.Exception;
            }
            Task.Factory.StartNew(() => { Process(monitor); });
        }
    }
}