using System.Web;
using System.Web.Mvc;
using Jw.ApiMonitor.Core;
using Jw.ApiMonitor.Core.MvcFilter;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace Jw.ApiMonitor.WebTest
{
    public class FilterConfig
    {
        private static byte[] result = new byte[1024];
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            var mvcMonitor = new MvcMonitorAttribute("Jw.ApiMonitor.WebTest");
            mvcMonitor.Process = MonitorLogHandler;
            filters.Add(mvcMonitor);
        }

        public static void MonitorLogHandler(MonitorInfo log)
        {
            //设定服务器IP地址  
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(new IPEndPoint(ip, 7701)); // 配置服务器IP与端口  
            }
            catch
            {
                return;
            }
            //通过clientSocket接收数据  
            int receiveLength = clientSocket.Receive(result);
            //通过 clientSocket 发送数据  
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    clientSocket.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(log)));
                }
                catch
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    break;
                }
            }
        }
    }
}
