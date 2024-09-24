using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Think9.Controllers.Basic;
using Think9.Controllers.Web.Quartz;
using Think9.Repository;
using Think9.Util.Global;
using Think9.Util.Model;
using Module = Autofac.Module;

namespace Think9.Controllers.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Configuration = configuration;
            WebHostEnvironment = env;
            GlobalContext.LogWhenStart(env);
            GlobalContext.HostingEnvironment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //todo
            GlobalContext.SystemConfig = Configuration.GetSection("SystemConfig").Get<SystemConfig>();
            GlobalContext.Services = services;
            GlobalContext.Configuration = Configuration;
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            GlobalContext.SystemConfig = Configuration.GetSection("SystemConfig").Get<SystemConfig>();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            //services.Configure<ForwardedHeadersOptions>(options =>
            //{
            //    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            //});
            //解决session问题
            services.AddSession();
            //代替HttpContext.Current
            services.AddHttpContextAccessor();

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRazorPages();

            //不加这句会（视图中的中文被html编码）
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

            //内存缓存
            services.AddMemoryCache();
            //redis缓存
            //services.AddStackExchangeRedisCache(t =>
            //{
            //    t.Configuration = Configuration.GetValue<string>("Redis:Configuration");
            //    t.InstanceName = Configuration.GetValue<string>("Redis:InstanceName");
            //});
            services.AddDistributedMemoryCache();

            //配置session(session是根据上面cache来区分存储源地的)
            services.AddSession(opts =>
            {
                opts.IdleTimeout = TimeSpan.FromMinutes(60); //设置Session闲置超时时间(有效时间周期)
                opts.Cookie.Name = "think9web_cookie";
                opts.Cookie.HttpOnly = true;
            });

            //配置存储视图的默认区域文件夹
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.AreaViewLocationFormats.Clear();

                options.AreaViewLocationFormats.Add("/Areas/{2}/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Areas/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Views/{2}/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Views/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Areas/Shared/{0}.cshtml");
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddControllersWithViews()
               //配置json返回的格式
               .AddJsonOptions(options =>
               {
                   options.JsonSerializerOptions.PropertyNamingPolicy = null;
                   options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                   options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
               })
                // 将Controller加入到Services中
                .AddControllersAsServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //启用静态资源(无参默认wwwroot文件夹)
                //app.UseStaticFiles();
                //app.UseHttpsRedirection();
                //app.UseRouting();
                //app.UseAuthorization();
                //app.UseEndpoints(endpoints =>
                //{
                //    endpoints.MapControllers();
                //});

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

                app.UseAuthorization();
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Base/Login/Error");
            }
            else
            {
                app.UseExceptionHandler("/Login/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            //实时通讯跨域
            app.UseCors("CorsPolicy");
            if (WebHostEnvironment.IsDevelopment())
            {
                //GlobalContext.SystemConfig.Debug = true;
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error?msg=404");
            }

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")) //执行文件下的wwwroot文件夹
            });

            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{area:exists}/{controller=Login}/{action=Index}/{id?}");
                //endpoints.MapControllerRoute("default", "{area:exists}/{controller=Login}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("default", "{controller=Login}/{action=Index}/{id?}");
            });

            GlobalContext.RootServices = app.ApplicationServices;
        }

        #region autofac

        /// <summary>
        /// 这里添加的方法来用autofac注入服务
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new RmesAutoFacModule());

            //Controller中使用属性注入
            var controllerBaseType = typeof(Controller);
            builder.RegisterAssemblyTypes(typeof(Program).Assembly)
            .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
            .PropertiesAutowired();

            //注册html解析
            builder.RegisterInstance(HtmlEncoder.Create(UnicodeRanges.All)).SingleInstance();
            //注册特性
            builder.RegisterType(typeof(HandlerLoginAttribute)).InstancePerLifetimeScope();
        }

        public class RmesAutoFacModule : Module
        {
            private static Autofac.IContainer _container;

            protected override void Load(ContainerBuilder builder)
            {
                //配置quartz定时任务(单例)
                builder.RegisterType<JobCenter>()
                    .PropertiesAutowired()
                    .SingleInstance();

                builder.RegisterType<DbContext>()
                    .PropertiesAutowired()
                    .InstancePerDependency();

                //WebAPI只用引用services和repository的接口，不用引用实现的dll。
                //如需加载实现的程序集，将dll拷贝到bin目录下即可，不用引用dll
                //var iServices = Assembly.Load("Think9.IServices");
                //var services = Assembly.Load("Think9.Services");
                var iRepository = Assembly.Load("Think9.IRepository");
                var repository = Assembly.Load("Think9.Repository");

                //根据名称约定（数据访问层的接口和实现均以Repository结尾），实现数据访问接口和数据访问实现的依赖
                //builder.RegisterAssemblyTypes(iRepository, repository)
                //  .Where(t => t.Name.EndsWith("Repository"))
                //  .AsImplementedInterfaces().PropertiesAutowired()
                //  .InstancePerDependency();

                //var controllerBaseType = typeof(ControllerBase);
                //builder.RegisterAssemblyTypes(typeof(Program).Assembly)
                //    .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
                //    .PropertiesAutowired() // 允许属性注入
                //    .EnableClassInterceptors(); // 允许在Controller类上使用拦截器
            }

            public static Autofac.IContainer GetContainer()
            {
                return _container;
            }
        }

        #endregion autofac

        public class DateTimeConverter : JsonConverter<DateTime>
        {
            /// <summary>
            /// 获取或设置DateTime格式
            /// <para>默认为: yyyy-MM-dd HH:mm:ss</para>
            /// </summary>
            public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";

            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
                => DateTime.Parse(reader.GetString());

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
                => writer.WriteStringValue(value.ToString(this.DateTimeFormat));
        }
    }
}