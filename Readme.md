
## Trong controller có thể truy cập các phương thức sau như trong PageModel
    this.User;
    this.TempData;
    this.ViewBag;
    this.ViewData;
    this.Url

    this.HttpContext;
    this.Request;
    this.Response;
    this.RouteData;
## Logger được đăng ký vào DI ứng dụng mặc định vào ứng dụng ASP.NET
    Nên khi cần sử dụng chỉ cần Inject vào đối tượng cần làm.
## Logger chia làm 6 cấp độ
    Debug
    Information
    Warning 
    Error
    Critical
    None
## Serilog
    Khi viết log có thể được lưu ở AWS S3, AZURE, TELEGRAM...
## Action trong Controller
    Kiểu public
    Có thể trả về bất kì kiểu dữ liệu gì.
    Thường triển khai từ IAcytionResult.
    Nếu triển khai từ IActionResult có thể trả về nhiều dạng dữ liệu khác nhau
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
## Để lấy được đường dân của thư mục chương trình
    IWebHostEnvironment environment
    enviroment.ContentRootPath
## Trả về view
    Cách 1: View(Đường dẫn tuyệt dối đến file .cshtml);
    Cách 2: View(Model); Thiết lập thông tin thông qua model
    Cách 3: View(Chỉ ghi tên template engine)
    Cách 4: Vierw() : tên template phải trùng tên với Action, và foler phải trùng tên class chứa Action.
## Truyền thông tin đến view:
    Thông qua Model
    Thông qua ViewData : thiết lập dữ liệu qua key ["key"]
    Thông qua ViewBag  : ViewBag.<tên biến>
    Thông qua TempData : thiết lập dữ liệu qua key ["key"]
    
