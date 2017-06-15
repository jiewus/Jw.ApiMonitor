using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jw.ApiMonitor.WebTest.Models
{
    /// <summary>
    /// 监控日志对象
    /// </summary>
    public class WebApiMonitorLog
    {
        public string ControllerName
        {
            get;
            set;
        }
        public string ActionName
        {
            get;
            set;
        }

        public DateTime ExecuteStartTime
        {
            get;
            set;
        }
        public DateTime ExecuteEndTime
        {
            get;
            set;
        }
        /// <summary>
        /// 请求的Action 参数
        /// </summary>
        public IDictionary<string, object> ActionParams
        {
            get;
            set;
        }
        /// <summary>
        /// Http请求头
        /// </summary>
        public string HttpRequestHeaders
        {
            get;
            set;
        }

        /// <summary>
        /// 请求方式
        /// </summary>
        public string HttpMethod
        {
            get;
            set;
        }
        /// <summary>
        /// 请求的IP地址
        /// </summary>
        public string IP
        {
            get;
            set;
        }

        /// <summary>
        /// 获取监控指标日志
        /// </summary>
        /// <param name="mtype"></param>
        /// <returns></returns>
        public string GetLoginfo()
        {
            string Msg = @"
            Action执行时间监控：
            ControllerName：{0}Controller
            ActionName:{1}
            开始时间：{2}
            结束时间：{3}
            总 时 间：{4}毫秒
            Action参数：{5}
            Http请求头:{6}
            客户端IP：{7},
            HttpMethod:{8}
                    ";
            return string.Format(Msg,
                ControllerName,
                ActionName,
                ExecuteStartTime,
                ExecuteEndTime,
                (ExecuteEndTime - ExecuteStartTime).TotalMilliseconds,
                GetCollections(ActionParams),
                HttpRequestHeaders,
                IP,
                HttpMethod);
        }

        /// <summary>
        /// 获取Action 参数
        /// </summary>
        /// <param name="collections"></param>
        /// <returns></returns>
        public string GetCollections(IDictionary<string, object> collections)
        {
            string parameters = string.Empty;
            if (collections == null || collections.Count == 0)
            {
                return parameters;
            }
            foreach (string key in collections.Keys)
            {
                parameters += string.Format("{0}={1}&", key, collections[key]);
            }
            if (!string.IsNullOrWhiteSpace(parameters) && parameters.EndsWith("&"))
            {
                parameters = parameters.Substring(0, parameters.Length - 1);
            }
            return parameters;
        }

        /// <summary>
        /// 获取IP
        /// </summary>
        /// <returns></returns>
        public string GetIP()
        {
            string ip = string.Empty;
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"]))
                ip = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
            if (string.IsNullOrEmpty(ip))
                ip = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
            return ip;
        }
    }
}