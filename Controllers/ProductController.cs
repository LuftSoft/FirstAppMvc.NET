using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP.NETMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ASP.NETMVC.Controllers
{
    [Area("ProductManage")]
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly ILogger<ProductService> _Logger;

        public ProductController(ProductService productService, ILogger<ProductService> logger)
        {
            _productService = productService;
            _Logger = logger;
        }

        public IActionResult Index()
        {
            //var products = (from p in _productService orderby p.ID select p).ToList();
            return View();
        }
        [Route("/product/product-info/{id?}")]
        public IActionResult Product_info(int id)
        {
            var product = _productService.Where(p => p.ID == id).FirstOrDefault();
            return View("ProductInfo", product);
        }
    }
}