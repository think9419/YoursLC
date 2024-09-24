using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Net;
using System.Text;
using Think9.Util.Global;

namespace Think9.Util
{
    public class WebHelper
    {
        private HttpContext httpContext;

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
        public static void WriteSession(string key, string value)
        {
            if (key.IsEmpty())
                return;
            IHttpContextAccessor hca = GlobalContext.ServiceProvider?.GetService<IHttpContextAccessor>();
            hca?.HttpContext?.Session.SetString(key, value);
        }

        /// <summary>
        /// 读取Session的值
        /// </summary>
        /// <param name="key">Session的键名</param>
        public static string GetSession(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }
            IHttpContextAccessor hca = GlobalContext.ServiceProvider?.GetService<IHttpContextAccessor>();
            return hca?.HttpContext?.Session.GetString(key) ?? "";
        }

        /// <summary>
        /// 删除指定Session
        /// </summary>
        /// <param name="key">Session的键名</param>
        public static void RemoveSession(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            IHttpContextAccessor hca = GlobalContext.ServiceProvider?.GetService<IHttpContextAccessor>();
            hca?.HttpContext?.Session.Remove(key);
        }

        #endregion Session操作

        #region Cookie操作

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string strValue)
        {
            IHttpContextAccessor hca = GlobalContext.ServiceProvider?.GetService<IHttpContextAccessor>();
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(30);
            hca?.HttpContext?.Response.Cookies.Append(strName, strValue, option);
        }

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        /// <param name="strValue">过期时间(分钟)</param>
        public static void WriteCookie(string strName, string strValue, int expires)
        {
            IHttpContextAccessor hca = GlobalContext.ServiceProvider?.GetService<IHttpContextAccessor>();
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddMinutes(expires);
            hca?.HttpContext?.Response.Cookies.Append(strName, strValue, option);
        }

        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string strName)
        {
            IHttpContextAccessor hca = GlobalContext.ServiceProvider?.GetService<IHttpContextAccessor>();
            return hca?.HttpContext?.Request.Cookies[strName] ?? "";
        }

        /// <summary>
        /// 删除Cookie对象
        /// </summary>
        /// <param name="CookiesName">Cookie对象名称</param>
        public static void RemoveCookie(string CookiesName)
        {
            IHttpContextAccessor hca = GlobalContext.ServiceProvider?.GetService<IHttpContextAccessor>();
            hca?.HttpContext?.Response.Cookies.Delete(CookiesName);
        }

        #endregion Cookie操作

        #region HttpWebRequest(请求网络资源)

        /// <summary>
        /// 请求网络资源,返回响应的文本
        /// </summary>
        /// <param name="url">网络资源地址</param>
        public static string HttpWebRequest(string url)
        {
            return HttpWebRequest(url, string.Empty, Encoding.GetEncoding("utf-8"));
        }

        /// <summary>
        /// 请求网络资源,返回响应的文本
        /// </summary>
        /// <param name="url">网络资源Url地址</param>
        /// <param name="parameters">提交的参数,格式：参数1=参数值1&amp;参数2=参数值2</param>
        public static string HttpWebRequest(string url, string parameters)
        {
            return HttpWebRequest(url, parameters, Encoding.GetEncoding("utf-8"), true);
        }

        /// <summary>
        /// 请求网络资源,返回响应的文本
        /// </summary>
        /// <param name="url">网络资源地址</param>
        /// <param name="parameters">提交的参数,格式：参数1=参数值1&amp;参数2=参数值2</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="isPost">是否Post提交</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="cookie">Cookie容器</param>
        /// <param name="timeout">超时时间</param>
        public static string HttpWebRequest(string url, string parameters, Encoding encoding, bool isPost = false,
             string contentType = "application/x-www-form-urlencoded", CookieContainer cookie = null, int timeout = 120000)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = timeout;
            request.CookieContainer = cookie;
            if (isPost)
            {
                byte[] postData = encoding.GetBytes(parameters);
                request.Method = "POST";
                request.ContentType = contentType;
                request.ContentLength = postData.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(postData, 0, postData.Length);
                }
            }
            var response = (HttpWebResponse)request.GetResponse();
            string result;
            using (Stream stream = response.GetResponseStream())
            {
                if (stream == null)
                    return string.Empty;
                using (var reader = new StreamReader(stream, encoding))
                {
                    result = reader.ReadToEnd();
                }
            }
            return result;
        }

        #endregion HttpWebRequest(请求网络资源)

        #region 当前连接

        public static HttpContext HttpContext
        {
            get { return GlobalContext.ServiceProvider?.GetService<IHttpContextAccessor>().HttpContext; }
        }

        #endregion 当前连接
    }
}