using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data;
using ASP.NETMVC.Models;
using ASP.NETMVC.Models.Blog;
using ASP.NETMVC.Models.Product;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP.NETMVC.Areas.Database.Controllers
{
    [Area("Database")]
    [Route("database-manage/[action]")]
    public class DbManageController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        //Để xóa db  phải inject vào dịch vụ AppDb
        [TempData]
        public string StatusMessage { set; get; }
        private readonly AppDbContext _context;
        public DbManageController(AppDbContext context, UserManager<AppUser> user, RoleManager<IdentityRole> role)
        {
            _context = context;
            _userManager = user;
            _roleManager = role;
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
        [HttpPost]
        public async Task<IActionResult> SeedDataAsync()
        {

            var roleNames = typeof(RoleName).GetFields().ToList();
            foreach (var role in roleNames)
            {
                string rolename = (string)role.GetRawConstantValue();
                //Console.WriteLine($"role name {rolename}");
                var rfound = await _roleManager.FindByNameAsync(rolename);
                if (rfound == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(rolename));
                    Console.WriteLine("Tao role thanh cong" + rolename);
                }
            }
            //admin - admin@example.com - pass: admin123 - mailconfirm = true
            var useradmin = await _userManager.FindByNameAsync("admin");

            if (useradmin == null)
            {
                AppUser user = new AppUser()
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };
                await _userManager.CreateAsync(user, "admin123");
                // Console.WriteLine("Tao user thanh cong" + user.UserName);
                await _userManager.AddToRoleAsync(user, RoleName.Administrator);
            }
            SeedPostCategory();
            SeedProductCategory();
            //await _context.Database.MigrateAsync();
            StatusMessage = "Đã tạo admin user thành công!";
            return RedirectToAction(nameof(Index));
        }
        private void SeedPostCategory()
        {
            _context._categories.RemoveRange(_context._categories.Where(c => c.Description.Contains("[fakeData]")));
            _context.Post.RemoveRange(_context.Post.Where(p => p.Content.Contains("[fakeData]")));
            _context.SaveChanges();


            //tạo fake data
            var fakeCategory = new Faker<Category>();
            int cm = 1;
            fakeCategory.RuleFor(c => c.Title, fk => $"CM{cm++} " + fk.Lorem.Sentence(1, 2).Trim('.'));
            fakeCategory.RuleFor(c => c.Description, fk => fk.Lorem.Sentence(5) + "[fakeData]");
            fakeCategory.RuleFor(c => c.Slug, fk => fk.Lorem.Slug());

            var cate1 = fakeCategory.Generate();
            var cate11 = fakeCategory.Generate();
            var cate12 = fakeCategory.Generate();
            var cate2 = fakeCategory.Generate();
            var cate21 = fakeCategory.Generate();
            var cate22 = fakeCategory.Generate();


            cate11.ParentCategory = cate1;
            cate12.ParentCategory = cate1;
            cate21.ParentCategory = cate2;
            cate22.ParentCategory = cate2;

            var categories = new Category[] { cate1, cate11, cate12, cate2, cate21, cate22 };
            _context._categories.AddRange(categories);
            //START
            var rCateIndex = new Random();
            int bv = 1;

            var user = _userManager.GetUserAsync(this.User).Result;
            var fakePost = new Faker<Post>();
            fakePost.RuleFor(p => p.AuthorId, f => user.Id);
            fakePost.RuleFor(p => p.Content, f => f.Lorem.Paragraph(7) + "[fakeData]");
            fakePost.RuleFor(p => p.DateCreated, f => f.Date.Between(new DateTime(2011, 1, 1), new DateTime(2022, 9, 10)));
            fakePost.RuleFor(p => p.Description, f => f.Lorem.Sentence(3));
            fakePost.RuleFor(p => p.Slug, f => f.Lorem.Slug());
            fakePost.RuleFor(p => p.Title, f => $"Bài {bv++} " + f.Lorem.Sentence(3, 4).Trim('.'));

            List<Post> posts = new List<Post>();
            List<PostCategory> list_categories = new List<PostCategory>();
            for (int i = 1; i <= 40; i++)
            {
                var post = fakePost.Generate();
                post.DateUpdated = post.DateCreated;
                posts.Add(post);
                list_categories.Add(
                    new PostCategory()
                    {
                        Post = post,
                        Category = categories[rCateIndex.Next(5)]
                    }
                );
            }
            _context.AddRange(posts);
            _context.AddRange(list_categories);
            //END

            _context.SaveChanges();
        }

        //seed product 
        private void SeedProductCategory()
        {
            _context.Products.RemoveRange(_context.Products.Where(c => c.Content.Contains("[fakeData]")));
            _context.CategoryProducts.RemoveRange(_context.CategoryProducts.Where(p => p.Description.Contains("[fakeData]")));
            _context.SaveChanges();
            //_context.Post.RemoveRange(_context.Post.Where(p => p.Content.Contains("[fakeData]")));


            var fakeCategory = new Faker<CategoryProduct>();
            int cm = 1;
            fakeCategory.RuleFor(c => c.Title, fk => $"Nhóm sản phẩm {cm++} " + fk.Lorem.Sentence(1, 2).Trim('.'));
            fakeCategory.RuleFor(c => c.Description, fk => fk.Lorem.Sentence(5) + "[fakeData]");
            fakeCategory.RuleFor(c => c.Slug, fk => fk.Lorem.Slug());

            var cate1 = fakeCategory.Generate();
            var cate11 = fakeCategory.Generate();
            var cate12 = fakeCategory.Generate();
            var cate2 = fakeCategory.Generate();
            var cate21 = fakeCategory.Generate();
            var cate22 = fakeCategory.Generate();


            cate11.ParentCategory = cate1;
            cate12.ParentCategory = cate1;
            cate21.ParentCategory = cate2;
            cate22.ParentCategory = cate2;

            var categories = new CategoryProduct[] { cate1, cate11, cate12, cate2, cate21, cate22 };
            _context.CategoryProducts.AddRange(categories);
            //START
            var rCateIndex = new Random();
            int bv = 1;

            var user = _userManager.GetUserAsync(this.User).Result;
            var fakeProduct = new Faker<ProductModel>();
            fakeProduct.RuleFor(p => p.AuthorId, f => user.Id);
            fakeProduct.RuleFor(p => p.Content, f => f.Commerce.ProductDescription() + "[fakeData]");
            fakeProduct.RuleFor(p => p.DateCreated, f => f.Date.Between(new DateTime(2011, 1, 1), new DateTime(2022, 9, 10)));
            fakeProduct.RuleFor(p => p.Description, f => f.Lorem.Sentence(3));
            fakeProduct.RuleFor(p => p.Slug, f => f.Lorem.Slug());
            fakeProduct.RuleFor(p => p.Title, f => $"Sản phẩm {bv++} " + f.Commerce.ProductName());
            fakeProduct.RuleFor(p => p.Price, f => int.Parse(f.Commerce.Price(10000, 500000000, 0)));

            List<ProductModel> procs = new List<ProductModel>();
            List<ProductCategoryProduct> list_categories = new List<ProductCategoryProduct>();
            for (int i = 1; i <= 40; i++)
            {
                var proc = fakeProduct.Generate();
                proc.DateUpdated = proc.DateCreated;
                procs.Add(proc);
                list_categories.Add(
                    new ProductCategoryProduct()
                    {
                        Product = proc,
                        Category = categories[rCateIndex.Next(5)]
                    }
                );
            }
            _context.AddRange(procs);
            _context.AddRange(list_categories);
            //END

            _context.SaveChanges();
        }
    }
}