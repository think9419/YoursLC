using CSRedis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Think9.Util.Model;

namespace Think9.Util.Global
{
    public class GlobalContext
    {
        /// <summary>
        /// 根服务
        /// </summary>
        public static IServiceProvider RootServices { get; set; }

        public static HttpContext HttpContext => RootServices?.GetService<IHttpContextAccessor>()?.HttpContext;

        /// <summary>
        /// All registered service and class instance container. Which are used for dependency injection.
        /// </summary>
        public static IServiceCollection Services { get; set; }

        public static IWebHostEnvironment HostingEnvironment { get; set; }

        /// <summary>
        /// Configured service provider.
        /// </summary>
        public static IServiceProvider ServiceProvider { get; set; }

        public static SystemConfig SystemConfig { get; set; }

        public static IConfiguration Configuration { get; set; }

        public static string GetVersion()
        {
            Version version = Assembly.GetEntryAssembly().GetName().Version;
            return version.Major + "." + version.Minor;
        }

        /// <summary>
        /// 程序启动时，记录目录
        /// </summary>
        /// <param name="env"></param>
        public static void LogWhenStart(IWebHostEnvironment env)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("程序启动");
            sb.AppendLine("ContentRootPath:" + env.ContentRootPath);
            sb.AppendLine("WebRootPath:" + env.WebRootPath);
            sb.AppendLine("IsDevelopment:" + env.IsDevelopment());
            Think9.Util.Helper.LogHelper.WriteWithTime(sb.ToString());
        }
    }
}