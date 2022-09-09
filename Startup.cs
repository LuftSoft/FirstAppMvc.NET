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
using ASP.NETMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using App.Services;
using App.Data;

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
            //Dang ky dich vu email
            services.AddOptions();
            var mailSetting = Configuration.GetSection("MailSettings");
            services.Configure<MailSettings>(mailSetting);
            services.AddSingleton<IEmailSender, SendMailService>();
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
            services.AddDbContext<AppDbContext>(options =>
            {
                string connectionString = Configuration.GetConnectionString("AppDbContext");
                options.UseSqlServer(connectionString);
            });
            //da inject UserManager, RoleManager
            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            // Truy cập IdentityOptions
            services.Configure<IdentityOptions>(options =>
            {
                // Thiết lập về Password
                options.Password.RequireDigit = false; // Không bắt phải có số
                options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
                options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
                options.Password.RequireUppercase = false; // Không bắt buộc chữ in
                options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
                options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                // Cấu hình Lockout - khóa user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
                options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 3 lầ thì khóa
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình về User.
                options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;  // Email là duy nhất


                // Cấu hình đăng nhập.
                options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
                options.SignIn.RequireConfirmedAccount = true;

            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login/";
                options.LogoutPath = "/logout/";
                options.AccessDeniedPath = "/khongduoctruycap.html";
            });

            services.AddAuthentication()
                    .AddGoogle(options =>
                    {
                        var gconfig = Configuration.GetSection("Authentication:Google");
                        options.ClientId = gconfig["ClientID"];
                        options.ClientSecret = gconfig["ClientSecrect"];
                        // https://localhost:5001/signin-google
                        options.CallbackPath = "/dang-nhap-tu-google";
                    })
                    .AddFacebook(options =>
                    {
                        var fconfig = Configuration.GetSection("Authentication:Facebook");
                        options.AppId = fconfig["AppID"];
                        options.AppSecret = fconfig["AppSecrect"];
                        options.CallbackPath = "/dang-nhap-tu-facebook";
                    })
                    // .AddTwitter()
                    // .AddMicrosoftAccount()
                    ;
            //error
            services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ViewManageMenu", builder =>
                {
                    builder.RequireAuthenticatedUser();
                    builder.RequireRole(RoleName.Administrator);
                });
            });

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
