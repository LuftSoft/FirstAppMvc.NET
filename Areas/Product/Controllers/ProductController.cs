using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP.NETMVC.Models;
using ASP.NETMVC.Models.Blog;
using Microsoft.AspNetCore.Authorization;
using App.Data;
using App.Models;
using Microsoft.AspNetCore.Identity;
using ASP.NETMVC.Areas.Blog.Models;
using App.Utilities;
using ASP.NETMVC.Areas.Product.Models;
using ASP.NETMVC.Models.Product;

namespace ASP.NETMVC.Areas.Product
{
    [Area("Product")]
    [Route("admin/product/[action]/{id?}")]
    [Authorize(Roles = RoleName.Administrator)]
    public class ProductController : Controller
    {
        [TempData]
        public string StatusMessage { set; get; }
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ProductController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Post
        //Sửa lại index sắp xếp theo ngày cập nhật
        public async Task<IActionResult> Index([FromQuery(Name = "p")] int currentPage, int pagesize)
        {
            var procs = _context.Products
                                .Include(p => p.Author)
                                .OrderByDescending(p => p.DateUpdated);
            int totalPost = procs.Count();
            if (pagesize <= 0) pagesize = 10;
            int countPages = (int)Math.Ceiling((double)totalPost / pagesize);

            if (currentPage > countPages) currentPage = countPages;
            if (currentPage < 1) currentPage = 1;

            var pagemodel = new PagingModel()
            {
                countpages = countPages,
                currentpage = currentPage,
                generateUrl = (pageNumber) => Url.Action("Index", new
                {
                    p = pageNumber,
                    pagesize = pagesize
                })

            };
            ViewBag.paggingmodel = pagemodel;
            ViewBag.totalpost = totalPost;
            ViewBag.postindex = (currentPage - 1) * pagesize;
            var postInPage = await procs.Skip((currentPage - 1) * pagesize)
                        .Take(pagesize)
                        .Include(p => p.ProductCategoryProduct)
                        .ThenInclude(pc => pc.Category)
                        .ToListAsync();
            return View(postInPage);
        }

        // GET: Post/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var procs = await _context.Products
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (procs == null)
            {
                return NotFound();
            }

            return View(procs);
        }

        // GET: Post/Create
        public async Task<IActionResult> Create()
        {
            var cates = await _context.CategoryProducts.ToListAsync();

            ViewData["categories"] = new MultiSelectList(cates, "Id", "Title");
            return View();
        }

        // POST: Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Slug,Content,Price,Published,categoriesID")] CreateProductModel proc)
        {
            var cates = await _context.CategoryProducts.ToListAsync();
            ViewData["categories"] = new MultiSelectList(cates, "Id", "Title");

            if (proc.Slug == null)
            {
                App.Utilities.AppUtilities.GenerateSlug(proc.Title, true);
            }
            //kiểm tra slug trùng k
            if (await _context.Products.AnyAsync(slug => slug.Slug == proc.Slug))
            {
                ModelState.AddModelError(string.Empty, "Nhập trùng url cho sản phẩm");
            };

            var user = await _userManager.GetUserAsync(this.User);
            proc.DateCreated = proc.DateUpdated = DateTime.Now;
            proc.AuthorId = user.Id;
            if (ModelState.IsValid)
            {
                _context.Add(proc);

                if (proc.categoriesID != null)
                {
                    foreach (var p in proc.categoriesID)
                    {
                        _context.Add(new ProductCategoryProduct()
                        {
                            CategoryID = p,
                            Product = proc
                        });
                    }
                }
                await _context.SaveChangesAsync();
                StatusMessage = "Vừa tạo sản phẩm mới";
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", proc.AuthorId);
            return View(proc);
        }

        // GET: Post/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var cates = await _context.CategoryProducts.ToListAsync();
            ViewData["categories"] = new MultiSelectList(cates, "Id", "Title");
            if (id == null)
            {
                return NotFound();
            }

            var proc = await _context.Products.Include(p => p.ProductCategoryProduct).FirstOrDefaultAsync(p => p.ProductId == id);
            if (proc == null)
            {
                return NotFound();
            }
            var procEdit = new CreateProductModel()
            {
                ProductId = proc.ProductId,
                Title = proc.Title,
                Content = proc.Content,
                Description = proc.Description,
                Slug = proc.Slug,
                Price = proc.Price,
                Published = proc.Published,
                categoriesID = proc.ProductCategoryProduct.Select(c => c.CategoryID).ToArray()
            };
            return View(procEdit);
        }

        // POST: Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Title,Description,Slug,Content,Price,Published,categoriesID")] CreateProductModel proc)
        {
            var cates = await _context.CategoryProducts.ToListAsync();
            ViewData["categories"] = new MultiSelectList(cates, "Id", "Title");
            Console.WriteLine(proc.ProductId);
            Console.WriteLine(proc.Title);
            if (id != proc.ProductId)
            {
                return NotFound("Không tìm thấy sản phẩm");
            }

            if (proc.Slug == null)
            {
                App.Utilities.AppUtilities.GenerateSlug(proc.Title, true);
            }
            //kiểm tra slug trùng k
            if (await _context.Products.AnyAsync(slug => slug.Slug == proc.Slug && proc.ProductId != id))
            {
                ModelState.AddModelError(string.Empty, "Nhập trùng url cho sản phẩm");
            };
            if (ModelState.IsValid)
            {
                try
                {
                    var procUpdate = await _context.Products.Include(p => p.ProductCategoryProduct).FirstOrDefaultAsync(p => p.ProductId == id);
                    if (procUpdate == null) return NotFound();
                    procUpdate.Title = proc.Title;
                    procUpdate.Content = proc.Content;
                    procUpdate.Description = proc.Description;
                    procUpdate.Slug = proc.Slug;
                    procUpdate.Price = proc.Price;
                    procUpdate.Published = proc.Published;
                    procUpdate.DateUpdated = DateTime.Now;
                    if (proc.categoriesID == null)
                    {
                        proc.categoriesID = new int[] { };
                    }
                    var oldCates = procUpdate.ProductCategoryProduct.Select(p => p.CategoryID).ToArray();
                    var newCates = proc.categoriesID;
                    var revCates = from p in procUpdate.ProductCategoryProduct
                                   where (!newCates.Contains(p.CategoryID))
                                   select p;
                    var addCates = from c in newCates
                                   where !(oldCates.Contains(c))
                                   select c;
                    _context.ProductCategoryProducts.RemoveRange(revCates);
                    foreach (var item in addCates)
                    {
                        _context.ProductCategoryProducts.Add(new ProductCategoryProduct() { CategoryID = item, ProductID = id });
                    }
                    _context.Update(procUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(proc.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                StatusMessage = "Vừa mới update";
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", proc.AuthorId);
            return View(proc);
        }

        // GET: Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proc = await _context.Products
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (proc == null)
            {
                return NotFound();
            }

            return View(proc);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var proc = await _context.Products.FindAsync(id);
            if (proc == null) return NotFound();
            _context.Products.Remove(proc);
            await _context.SaveChangesAsync();
            StatusMessage = "Đã xóa thành công!";

            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
