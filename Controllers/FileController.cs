using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ASP.NETMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ASP.NETMVC.Controllers
{
    public class FileController : Controller
    {
        private readonly ILogger<FileController> _logger;
        private readonly ProductService _productService;
        [TempData]
        public string _statusMessage { set; get; }
        public FileController(ILogger<FileController> logger, ProductService PService)
        {
            _logger = logger;
            _productService = PService;
        }
        public IActionResult Img()
        {
            _logger.LogInformation("-->Begin Img action");
            string filePath = Path.Combine(Startup.ContentRootPath, "Files", "devon_aoki.jpg");
            var bytes = System.IO.File.ReadAllBytes(filePath);

            return File(bytes, "image/jpg");
        }
        public IActionResult returnJson()
        {
            return Json(new { Name = "Iphone 14 Pro", Price = "1000$" });
        }
        public IActionResult returnViewHello(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                username = "Khách";
            }
            return View("xinchao", username);
        }

        public IActionResult ViewProduct(int id)
        {
            var products = _productService.Where(p => p.ID == id).FirstOrDefault();
            if (products == null)
            {
                _statusMessage = "Sản phẩm bạn yêu cầu không tồn tại";
                return Redirect(Url.Action("Index", "Home"));
            }
            //View//File//ViewProduct
            //MyView//File//ViewProduct
            this.ViewData["product"] = products;
            //truyền ttin qua view bag
            /*
            this.ViewBag.product = products;
            return View("ViewProduct2");
             
            truyển thông qua view data
            this.ViewData["product"] = products;
            return View("ViewProduct1");
            */
            this.ViewBag.product = products;
            return View("ViewProduct2");
        }
    }
}