using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP.NETMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ASP.NETMVC.Models.Blog;
using Microsoft.EntityFrameworkCore;
using App.Models;

namespace ASP.NETMVC.Areas.Product
{
    [Area("Product")]
    public class ViewPostController : Controller
    {
        private readonly ILogger<ViewPostController> _logger;
        private readonly AppDbContext _context;


        public ViewPostController(ILogger<ViewPostController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        private List<Category> GetCategory()
        {
            var categories = _context._categories
                            .Include(c => c.CategoryChildren)
                            .AsEnumerable()
                            .Where(c => c.ParentCategory == null).ToList();
            return categories;
        }
        [Route("/post/{categoryslug?}")]
        public IActionResult Index(string categoryslug, int _page, [FromQuery(Name = "p")] int currentPage, int pagesize)
        {
            var categories = GetCategory();
            Category category = null;
            if (!string.IsNullOrEmpty(categoryslug))
            {
                category = _context._categories.Where(c => c.Slug == categoryslug)
                                                .Include(c => c.CategoryChildren)
                                                .FirstOrDefault();
                if (category == null) return NotFound("eror when get category");
            }
            var post = _context.Post.Include(p => p.Author)
                                    .Include(p => p.PostCategories)
                                    .ThenInclude(p => p.Category)
                                    .AsQueryable().OrderByDescending(p => p.DateUpdated);
            if (category != null)
            {
                var ids = new List<int>();
                category.ChildCategories(ids, null);
                ids.Add(category.Id);
                post = (IOrderedQueryable<Post>)post.Where(p => p.PostCategories.Where(pc => ids.Contains(pc.CategoryID)).Any());
            }
            //Phân trang
            int totalPost = post.Count();
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

            ViewBag.Category = category;
            ViewBag.categories = categories;
            ViewBag.cSlug = categoryslug;
            ViewBag.paggingmodel = pagemodel;
            ViewBag.totalpost = totalPost;
            ViewBag.postindex = (currentPage - 1) * pagesize;
            var postInPage = post.Skip((currentPage - 1) * pagesize)
                        .Take(pagesize);
            //phân trang
            return View(postInPage.ToList());
        }
        [Route("/post/{postslug}.html")]
        public IActionResult Detail(string postslug)
        {
            var categories = GetCategory();
            ViewBag.categories = categories;

            var _post = _context.Post.Where(p => p.Slug == postslug)
                                        .Include(p => p.Author)
                                        .Include(p => p.PostCategories)
                                        .ThenInclude(pc => pc.Category)
                                        .FirstOrDefault();
            if (_post == null) return NotFound("Không tìm thấy bài viết");
            Category category = _post.PostCategories.FirstOrDefault()?.Category;
            ViewBag.category = category;
            var relativePost = _context.Post.Where(p => p.PostCategories.Any(c => c.CategoryID == category.Id))
                                                .Where(p => p.PostId != _post.PostId)
                                                .OrderByDescending(p => p.DateUpdated)
                                                .Take(5);
            ViewBag.relativePost = relativePost.ToList();
            return View(_post);
        }

    }
}