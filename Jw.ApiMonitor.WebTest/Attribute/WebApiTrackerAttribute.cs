using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Jw.ApiMonitor.WebTest.Models;

namespace Jw.ApiMonitor.WebTest.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class WebApiTrackerAttribute : ActionFilterAttribute//, ExceptionFilterAttribute  
    {
        private readonly string Key = "_thisWebApiOnActionMonitorLog_";
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            WebApiMonitorLog monLog = new WebApiMonitorLog();
            monLog.ExecuteStartTime = DateTime.Now;
            //获取Action 参数
            monLog.ActionParams = actionContext.ActionArguments;
            monLog.HttpRequestHeaders = actionContext.Request.Headers.ToString();
            monLog.HttpMethod = actionContext.Request.Method.Method;

            actionContext.Request.Properties[Key] = monLog;
            var form = System.Web.HttpContext.Current.Request.Form;
            #region 如果参数是实体对象，获取序列化后的数据
            Stream stream = actionContext.Request.Content.ReadAsStreamAsync().Result;
            Encoding encoding = Encoding.UTF8;
            stream.Position = 0;
            string responseData = "";
            using (StreamReader reader = new StreamReader(stream, encoding))
            {
                responseData = reader.ReadToEnd().ToString();
            }
            if (!string.IsNullOrWhiteSpace(responseData) && !monLog.ActionParams.ContainsKey("__EntityParamsList__"))
            {
                monLog.ActionParams["__EntityParamsList__"] = responseData;
            }
            #endregion
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            WebApiMonitorLog monLog = actionExecutedContext.Request.Properties[Key] as WebApiMonitorLog;
            monLog.ExecuteEndTime = DateTime.Now;
            monLog.ActionName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
            monLog.ControllerName = actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            NLog.LogManager.GetLogger("DefaultLog").Info(monLog.GetLoginfo());
            if (actionExecutedContext.Exception != null)
            {
                string Msg = string.Format(@"
                请求【{0}Controller】的【{1}】产生异常：
                Action参数：{2}
               Http请求头:{3}
                客户端IP：{4},
                HttpMethod:{5}
                    ", monLog.ControllerName, monLog.ActionName, monLog.GetCollections(monLog.ActionParams), monLog.HttpRequestHeaders, monLog.GetIP(), monLog.HttpMethod);
                NLog.LogManager.GetLogger("DefaultLog").Error(actionExecutedContext.Exception, Msg);
            }
        }

    }
}