using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace Think9.Controllers.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                throw;
            }
            finally
            {
                //NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(webBuilder =>
            {
                string port = Think9.Services.Base.Configs.GetValue("port");//¶Ë¿ÚºÅ
                webBuilder.UseUrls("http://*:" + port);//¶Ë¿Ú
                webBuilder.UseStartup<Startup>();
            });
    }
}