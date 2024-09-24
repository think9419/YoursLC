using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Think9.Util.Global;

namespace Think9.Services.Base
{
    public class WebHelper
    {
        private HttpContext httpContext;

        #region 当前连接

        public static HttpContext HttpContext
        {
            get { return GlobalContext.ServiceProvider?.GetService<IHttpContextAccessor>().HttpContext; }
        }

        #endregion 当前连接

        public WebHelper(HttpContext context)
        {
            httpContext = context;
        }

        #region Session操作

        /// <summary>
        /// 写Session
        /// </summary>
        /// <param name="key">Session的键名</param>
        /// <param name="value">Session的键值</param>
        public void WriteSession(string key, string value)
        {
            httpContext.Session.SetString(key, value);
        }

        /// <summary>
        /// 读取Session的值
        /// </summary>
        /// <param name="key">Session的键名</param>
        public string GetSession(string key)
        {
            bool rs = httpContext.Session.TryGetValue(key, out byte[] val);
            string value = "";
            if (rs)
            {
                value = Encoding.UTF8.GetString(val);
                return value;
            }
            value = "";
            return value;
        }

        /// <summary>
        /// 删除指定Session
        /// </summary>
        /// <param name="key">Session的键名</param>
        public void RemoveSession(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;
            httpContext.Session.Remove(key);
        }

        /// <summary>
        /// 清空session
        /// </summary>
        /// <param name="httpContext"></param>
        public void ClearSession()
        {
            httpContext.Session.Clear();
        }

        #endregion Session操作

        #region Cookie操作

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        public void WriteCookie(string strName, string strValue, int minutes = 60)
        {
            httpContext.Response.Cookies.Append(strName, strValue, new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(minutes)
            });
        }

        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        public string GetCookie(string strName)
        {
            var res = httpContext.Request.Cookies.TryGetValue(strName, out string value);
            string reslut = "";
            if (res)
                return value;
            return reslut;
        }

        /// <summary>
        /// 删除Cookie对象
        /// </summary>
        /// <param name="CookiesName">Cookie对象名称</param>
        public void RemoveCookie(string CookiesName)
        {
            httpContext.Response.Cookies.Delete(CookiesName);
        }

        #endregion Cookie操作

        public static string GetIP()
        {
            string ip = "";
            try
            {
                using (WebClient wc = new WebClient())
                {
                    ip = wc.DownloadString("http://ipinfo.io/ip");
                }
            }
            catch (Exception ex)
            {
                ip = "0.0.0.1";
            }

            return ip;
        }

        /// <summary>
        /// 根据Ip获取我们所要的信息
        /// </summary>
        /// <param name="strIp"></param>
        /// <returns></returns>
        public static string GetstringIpAddress(string strIp)
        {
            return GetAddress("https://www.ip138.com/iplookup.asp?ip=" + strIp + "&action=2", "gb2312");
        }

        /// <summary>
        /// 抓取网页查询信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding">编码类型</param>
        /// <returns></returns>
        public static string GetAddress(string url, string encoding)
        {
            string pagehtml = string.Empty;
            string Address = "";

            try
            {
                using (WebClient MyWebClient = new WebClient())
                {
                    //控制台应用和.net core 需要这一句，需要安装NetGet包System.Text.Encoding.CodePages
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    Encoding encode = Encoding.GetEncoding(encoding);
                    MyWebClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.84 Safari/537.36");
                    MyWebClient.Credentials = CredentialCache.DefaultCredentials;
                    Byte[] pageData = MyWebClient.DownloadData(url);
                    pagehtml = encode.GetString(pageData);

                    string pre = "var ip_result = {\"ASN归属地\":\"";
                    int pos = pagehtml.IndexOf(pre);
                    pagehtml = pagehtml.Substring(pos + pre.Length);
                    pagehtml = pagehtml.Substring(0, pagehtml.IndexOf('"'));
                    //string[] arr = pagehtml.Split(new char[] { '省', '市', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    //for (int i = 0; i < arr.GetLength(0); i++)
                    //{
                    //    if (arr[i] != null)
                    //    {
                    //        Address += " " + arr[i].ToString();
                    //    }
                    //}
                    Address = pagehtml;
                }

                return Address;
            }
            catch (Exception ex)
            {
                return "其他";
            }
        }

        private static async Task<string> GetPublicIP(HttpClient client)
        {
            string response = await client.GetStringAsync("http://ipinfo.io/ip");
            string ip = ExtractIP(response);
            return ip;
        }

        private static string ExtractIP(string response)
        {
            JObject json = JObject.Parse(response);
            return json["ip"].ToString();
        }

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