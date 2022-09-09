using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP.NETMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP.NETMVC.Areas.Database.Controllers
{
    [Area("Database")]
    [Route("database-manage/[action]")]
    public class DbManageController : Controller
    {
        //Để xóa db  phải inject vào dịch vụ AppDb
        [TempData]
        public string StatusMessage { set; get; }
        private readonly AppDbContext _context;
        public DbManageController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult DeleteDb()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteDbAsync()
        {
            var rel = await _context.Database.EnsureDeletedAsync();
            StatusMessage = rel ? "Đã xóa database thành công!" : "Không xóa đc db";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Migrate()
        {
            await _context.Database.MigrateAsync();
            StatusMessage = "Đã tạo database thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}