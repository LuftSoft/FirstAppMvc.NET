using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ASP.NETMVC.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using ASP.NETMVC.ExtendMethod;
using Microsoft.AspNetCore.Routing.Constraints;

namespace ASP.NETMVC
{
    public class Startup
    {
        public static string ContentRootPath { set; get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            ContentRootPath = environment.ContentRootPath;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            //{0} ten Action
            //{1} ten Controller
            //{2} ten Area

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationFormats.Add("/MyView/{1}/{0}" + RazorViewEngine.ViewExtension);
            });
            services.AddSingleton<ProductService>();
            services.AddSingleton<PlanetService>();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.AddStatusCode(); //Tùy biển response using ASP.NETMVC.ExtendMethod;

            app.UseRouting();

            app.UseAuthentication(); //Xac định danh tính
            app.UseAuthorization(); //Xác định quyền truy cập

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "xemsp",
                    pattern: "{url}/{id?}",
                    defaults: new
                    {
                        controller = "File",
                        action = "ViewProduct"
                    },
                    constraints: new
                    {
                        url = new RegexRouteConstraint(@"^((xemsanpham)|(viewproduct))$"),
                        //hoặc url = "xemsanpham"
                        id = new RangeRouteConstraint(1, 3)
                    });
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute(
                    name: "product",
                    pattern: "/{controller}/{action=Index}/{id?}",
                    areaName: "ProductManage"
                );
                endpoints.MapRazorPages();

                //URL = start-here
                //controller
                //action 
                //area
                // endpoints.MapControllerRoute(
                //     name: "firstroute",
                //     pattern: "start-here/{id}",
                //     defaults: new
                //     {
                //         controller = "File",
                //         action = "ViewProduct",
                //         id = 3
                //     }
                //     );
            });
        }
    }
}
