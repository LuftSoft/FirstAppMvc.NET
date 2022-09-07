
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



    
    


