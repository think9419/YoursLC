using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using Think9.Models;
using Think9.Util.Global;

namespace Think9.Controllers.Basic
{
    public class WebIp
    {
        public List<valueTextEntity> GetOrderList()
        {
            List<valueTextEntity> list = new List<valueTextEntity>();
            list.Add(new valueTextEntity { Value = "", Text = "" });
            return list;
        }

        #region 当前连接

        public static HttpContext HttpContext
        {
            get { return GlobalContext.HttpContext; }
        }

        #endregion 当前连接

        #region 网络信息

        public static string Ip
        {
            get
            {
                //var httpContext = HelperHttpContext.Current;
                string result = string.Empty;
                try
                {
                    if (HttpContext != null)
                    {
                        result = GetWebClientIp();
                    }
                    if (string.IsNullOrEmpty(result))
                    {
                        result = GetLanIp();
                    }
                }
                catch (Exception)
                {
                    return string.Empty;
                }
                return result;
            }
        }

        private static string GetWebClientIp()
        {
            try
            {
                string ip = GetWebRemoteIp();
                foreach (var hostAddress in Dns.GetHostAddresses(ip))
                {
                    if (hostAddress.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return hostAddress.ToString();
                    }
                    else if (hostAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        return hostAddress.MapToIPv4().ToString();
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return string.Empty;
        }

        public static string GetLanIp()
        {
            try
            {
                foreach (var hostAddress in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    if (hostAddress.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return hostAddress.ToString();
                    }
                    else if (hostAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        return hostAddress.MapToIPv4().ToString();
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return string.Empty;
        }

        private static string GetWebRemoteIp()
        {
            try
            {
                string ip = HttpContext?.Connection?.RemoteIpAddress == null ? string.Empty : HttpContext?.Connection?.RemoteIpAddress.ToString();
                if (HttpContext != null && HttpContext.Request != null)
                {
                    if (HttpContext.Request.Headers.ContainsKey("X-Real-IP"))
                    {
                        ip = HttpContext.Request.Headers["X-Real-IP"].ToString();
                    }

                    if (HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
                    {
                        ip = HttpContext.Request.Headers["X-Forwarded-For"].ToString();
                    }
                }
                return ip;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        #endregion 网络信息

        #region IP位置查询

        public static string GetIpLocation(string ipAddress)
        {
            string ipLocation = "未知";
            try
            {
                if (!IsInnerIP(ipAddress))
                {
                    ipLocation = GetIpLocationFromPCOnline(ipAddress);
                }
                else
                {
                    ipLocation = "本地局域网";
                }
            }
            catch (Exception)
            {
                return ipLocation;
            }
            return ipLocation;
        }

        private static string GetIpLocationFromPCOnline(string ipAddress)
        {
            string ipLocation = "未知";
            try
            {
                var res = "";
                using (var client = new HttpClient())
                {
                    var URL = "http://whois.pconline.com.cn/ip.jsp?ip=" + ipAddress;
                    var response = client.GetAsync(URL).GetAwaiter().GetResult();
                    response.EnsureSuccessStatusCode();
                    res = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }
                if (!string.IsNullOrEmpty(res))
                {
                    ipLocation = res.Trim();
                }
            }
            catch
            {
                ipLocation = "未知";
            }
            return ipLocation;
        }

        #endregion IP位置查询

        #region 判断是否是外网IP

        public static bool IsInnerIP(string ipAddress)
        {
            bool isInnerIp = false;
            long ipNum = GetIpNum(ipAddress);
            /**
                私有IP：A类 10.0.0.0-10.255.255.255
                            B类 172.16.0.0-172.31.255.255
                            C类 192.168.0.0-192.168.255.255
                当然，还有127这个网段是环回地址
           **/
            long aBegin = GetIpNum("10.0.0.0");
            long aEnd = GetIpNum("10.255.255.255");
            long bBegin = GetIpNum("172.16.0.0");
            long bEnd = GetIpNum("172.31.255.255");
            long cBegin = GetIpNum("192.168.0.0");
            long cEnd = GetIpNum("192.168.255.255");
            isInnerIp = IsInner(ipNum, aBegin, aEnd) || IsInner(ipNum, bBegin, bEnd) || IsInner(ipNum, cBegin, cEnd) || ipAddress.Equals("127.0.0.1");
            return isInnerIp;
        }

        /// <summary>
        /// 把IP地址转换为Long型数字
        /// </summary>
        /// <param name="ipAddress">IP地址字符串</param>
        /// <returns></returns>
        private static long GetIpNum(string ipAddress)
        {
            string[] ip = ipAddress.Split('.');
            long a = int.Parse(ip[0]);
            long b = int.Parse(ip[1]);
            long c = int.Parse(ip[2]);
            long d = int.Parse(ip[3]);

            long ipNum = a * 256 * 256 * 256 + b * 256 * 256 + c * 256 + d;
            return ipNum;
        }

        private static bool IsInner(long userIp, long begin, long end)
        {
            return (userIp >= begin) && (userIp <= end);
        }

        #endregion 判断是否是外网IP
    }
}