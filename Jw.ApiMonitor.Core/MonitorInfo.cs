using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web;

namespace Jw.ApiMonitor.Core
{
    public class MonitorInfo
    {
        /// <summary>
        /// 当前请求的数据上下文
        /// </summary>
        internal HttpContext HttpContext { get; set; }

        /// <summary>
        /// 当前请求过来的Id，由系统生成。如果没有的话则直接生成一个（GUID）
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// 当前服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 当前服务部署ip
        /// </summary>
        public string DeployIpAddr => GetIpAddr();

        /// <summary>
        /// 当前请求的Url地址
        /// </summary>
        public string RequestUrl { get; set; }

        /// <summary>
        /// 当前请求方法执行开始时间
        /// </summary>
        public DateTime ExecuteStartTime
        {
            get;
            set;
        }

        /// <summary>
        /// 当前请求方法执行结束时间
        /// </summary>
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
        public string IpAddr
        {
            get;
            set;
        }

        /// <summary>
        /// 当前请求的状态码
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// 当前请求的错误信息
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 获取当前服务部署的ip地址
        /// </summary>
        /// <returns></returns>
        private string GetIpAddr()
        {
            string ip = string.Empty;
            if (!string.IsNullOrEmpty(HttpContext.Request.ServerVariables["HTTP_VIA"]))
                ip = Convert.ToString(HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
            if (string.IsNullOrEmpty(ip))
                ip = Convert.ToString(HttpContext.Request.ServerVariables["REMOTE_ADDR"]);
            return ip;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
