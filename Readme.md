
## Trong controller có thể truy cập các phương thức sau như trong PageModel
- this.User;
- this.TempData;
- this.ViewBag;
- this.ViewData;
- this.Url
- this.HttpContext;
- this.Request;
- this.Response;
- this.RouteData;
## Logger được đăng ký vào DI ứng dụng mặc định vào ứng dụng ASP.NET
    - Nên khi cần sử dụng chỉ cần Inject vào đối tượng cần làm.
## Logger chia làm 6 cấp độ
- Debug
- Information
- Warning 
- Error
- Critical
- None
## Serilog
    Khi viết log có thể được lưu ở AWS S3, AZURE, TELEGRAM...
## Action trong Controller
- Kiểu public
- Có thể trả về bất kì kiểu dữ liệu gì.
- Thường triển khai từ IAcytionResult.
- Nếu triển khai từ IActionResult có thể trả về nhiều dạng dữ liệu khác nhau
```
        Kiểu trả về                 | Phương thức
        ------------------------------------------------
        ContentResult               | Content()
        EmptyResult                 | new EmptyResult()
        FileResult                  | File()
        ForbidResult                | Forbid()
        JsonResult                  | Json()
        LocalRedirectResult         | LocalRedirect()
        RedirectResult              | Redirect()
        RedirectToActionResult      | RedirectToAction()
        RedirectToPageResult        | RedirectToRoute()
        RedirectToRouteResult       | RedirectToPage()
        PartialViewResult           | PartialView()
        ViewComponentResult         | ViewComponent()
        StatusCodeResult            | StatusCode()
        ViewResult                  | View()
```
## Để lấy được đường dân của thư mục chương trình
- IWebHostEnvironment environment
- enviroment.ContentRootPath
## Trả về view
- Cách 1: View(Đường dẫn tuyệt dối đến file .cshtml);
- Cách 2: View(Model); Thiết lập thông tin thông qua model
- Cách 3: View(Chỉ ghi tên template engine)
- Cách 4: Vierw() : tên template phải trùng tên với Action, và foler phải trùng tên class chứa Action.
## Truyền thông tin đến view:
- Thông qua Model
- Thông qua ViewData : thiết lập dữ liệu qua key ["key"]
- Thông qua ViewBag  : ViewBag.<tên biến>
- Thông qua TempData : thiết lập dữ liệu qua key ["key"]
# header1
**bold text**
*italic text*
***both bold and italic text***
[Link name](https://www.facebook.com/)
<https://www.facebook.com/>
## Route, 
### Endpoints map
- endpoints.MapControllerRoute
- endpoints.MapController
- endpoints.MapDefaultControllerRoute
- endpoints .MapAreaControllerRoute
```
Ví dụ một endpoints:
    endpoints.MapControllerRoute(
        name: "default", //tên endpoint
        pattern: "{controller=Home}/{action=Index}/{id?}");
        //cấu trúc : controller/action
```
**[IrouteConstraint](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.routing.irouteconstraint?view=aspnetcore-6.0)** ràng buộc cacsc tham số trong controller route : {Url},{tham số}, ..
## Các attribute được dùng cho action

- [AcceptVerb]
- [Route]
    ```
    - Có thể khai báo nhiều route cho 1 action
    - Order : Thiết lập độ ưu tiên cho route
    - Có thể thiết lập Route ở cấp độ controller
    [Route("[controller]-[action].html",Order=1)]
    [Route("name/[action]",Order=2)]
    ```
- [HttpGet]
- [HttpPost]
- [HttpPut]
- [HttpPatch]
- [HttpDelete]
## Areas
- Là tên dùng đê routing
- Là cấu trúc thư mục chứa M.V.C
- Thiết lập area cho controller bằng ```[Area("Area name")]```
- Tạo cấu trúc thư mục
```
dotnet aspnet-codegenerator area <Tên Area>
```
## Phát sinh Url cho Action bằng Url Helper
- Url.Action()
- Url.ActionLink()
## Lấy Url cho Route
- Url.RouteUrl()
- Url.Link()
- asp-all-route-data = "new Dictionary(){
    name = '',
    id = '',
    price = ''
    }"
##  Generate tự động các area, controller
- dotnet aspnet-codegenerator area Database
- dotnet aspnet-codegenerator controller
## Các thư viện cần thiết cho ASP.NET MVC
    <ItemGroup>  
        <!-- DI and console -->
        <PackageReference Include="Microsoft.Extensions.Logging.Console" Version="6.0.0" />
        <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="6.0.0" />
        <!-- code generator -->
        <PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="5.0.2" />
        <!-- EF Framework -->
        <PackageReference Include="System.Data.SqlClient" Version="4.8.3" />
        <PackageReference Include="Microsoft.EntityFrameworkCore" Version="5.0.9" />
        <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="5.0.9" />
        <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="5.0.9" />
        <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="5.0.9" />
        <!-- Identity -->
        <PackageReference Include="Microsoft.AspNetCore.Identity" Version="2.2.0" />
        <PackageReference Include="Microsoft.AspNetCore.Identity.UI" Version="5.0.9" />
        <PackageReference Include="Microsoft.AspNetCore.Authentication" Version="2.2.0" />
        <PackageReference Include="Microsoft.AspNetCore.Http.Abstractions" Version="2.2.0" />
        <PackageReference Include="Microsoft.AspNetCore.Authentication.oAuth" Version="2.2.0" />
        <PackageReference Include="Microsoft.AspNetCore.Authentication.Cookies" Version="2.2.0" />
        <PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="5.0.2" />
        <!-- Mail -->
        <PackageReference Include="MailKit" Version="2.6.0" />
        <PackageReference Include="MimeKit" Version="3.2.0" />
        <!--Dang nhap bang GG,FB,TWEET  -->
        <PackageReference Include="Microsoft.AspNetCore.Authentication.Google" Version="5.0.7" />
        <PackageReference Include="Microsoft.AspNetCore.Authentication.Facebook" Version="5.0.9" />
        <PackageReference Include="Microsoft.AspNetCore.Authentication.Twitter" Version="5.0.11" />
        <PackageReference Include="Microsoft.AspNetCore.Authentication.MicrosoftAccount" Version="5.0.7" />
        <!-- openID and JWT -->
        <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="5.0.12" />
        <PackageReference Include="Microsoft.AspNetCore.Authentication.OpenIdConnect" Version="5.0.9" />
    </ItemGroup>
## Thiết lập logging cho entity framework
    Đơn giản chỉ cẩn thêm các dòng setting vào app setting như
    "Microsoft.EntityFrameworkCore.Query": "Information",
    "Microsoft.EntityFrameworkCore.Database.Command": "Information"
**Ví dụ**
- **[Simple Logging EntityFramework Core](https://docs.microsoft.com/en-us/ef/core/logging-events-diagnostics/simple-logging)**
- appseting.json
## Xây dựng website blog đầu tiên
- Tạo table category
## Một số tips
- string.Concat(Enumerable.Repeat("&nbsp;&nbsp;&nbsp", level));
- @Html.Raw(prefix) <a asp-action="Edit" asp-route-id="@item.Id">(@level) @item.Title</a>
## Tích hợp trình soạn thảo vào HTML
- Một số trình soạn thảo phổ biến như
```
CkEditor, Summernote, TinyMCE ...
```