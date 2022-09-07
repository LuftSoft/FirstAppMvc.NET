using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace ASP.NETMVC.ExtendMethod
{
    public static class AppExtend
    {
        public static void AddStatusCode(this IApplicationBuilder app)
        {
            app.UseStatusCodePages(appErr =>
                        {
                            appErr.Run(async context =>
                            {
                                var response = context.Response;
                                var code = context.Response.StatusCode;
                                var content = @$"<html>
                                        <head>
                                            <meta charset='UTF-8'/>
                                            <title>
                                                Lỗi {code}
                                            </title>
                                        <head/>
                                        <body>
                                            <p style='color:red;'>Có lỗi xảy ra {code} - {(HttpStatusCode)code}<p/>
                                        </body>
                                    </html>";
                                await response.WriteAsync(content);
                            });
                        }); // Bắt các lỗi từ 400 đến 599, trả về response mặc định
        }
    }
}