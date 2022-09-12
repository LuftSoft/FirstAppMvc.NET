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

namespace ASP.NETMVC.Areas.Blog.Controllers
{
    [Area("Blog")]
    [Route("admin/blog/post/[action]/{id?}")]
    [Authorize(Roles = RoleName.Administrator)]
    public class PostController : Controller
    {
        [TempData]
        public string StatusMessage { set; get; }
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public PostController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Post
        //Sửa lại index sắp xếp theo ngày cập nhật
        public async Task<IActionResult> Index([FromQuery(Name = "p")] int currentPage, int pagesize)
        {
            var posts = _context.Post
                                .Include(p => p.Author)
                                .OrderByDescending(p => p.DateUpdated);
            int totalPost = posts.Count();
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
            var postInPage = await posts.Skip((currentPage - 1) * pagesize)
                        .Take(pagesize)
                        .Include(p => p.PostCategories)
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

            var post = await _context.Post
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Post/Create
        public async Task<IActionResult> Create()
        {
            var cates = await _context._categories.ToListAsync();

            ViewData["categories"] = new MultiSelectList(cates, "Id", "Title");
            return View();
        }

        // POST: Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Slug,Content,Published,categoriesID")] CreatePostModel post)
        {
            var cates = await _context._categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(cates, "Id", "Title");

            if (post.Slug == null)
            {
                App.Utilities.AppUtilities.GenerateSlug(post.Title, true);
            }
            //kiểm tra slug trùng k
            if (await _context.Post.AnyAsync(slug => slug.Slug == post.Slug))
            {
                ModelState.AddModelError(string.Empty, "Nhập trùng url cho bài viết");
            };

            var user = await _userManager.GetUserAsync(this.User);
            post.DateCreated = post.DateUpdated = DateTime.Now;
            post.AuthorId = user.Id;
            if (ModelState.IsValid)
            {
                _context.Add(post);

                if (post.categoriesID != null)
                {
                    foreach (var p in post.categoriesID)
                    {
                        _context.Add(new PostCategory()
                        {
                            CategoryID = p,
                            Post = post
                        });
                    }
                }
                await _context.SaveChangesAsync();
                StatusMessage = "Vừa tạo bài viết mới";
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", post.AuthorId);
            return View(post);
        }

        // GET: Post/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var cates = await _context._categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(cates, "Id", "Title");
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.Include(p => p.PostCategories).FirstOrDefaultAsync(p => p.PostId == id);
            if (post == null)
            {
                return NotFound();
            }
            var postEdit = new CreatePostModel()
            {
                PostId = post.PostId,
                Title = post.Title,
                Content = post.Content,
                Description = post.Description,
                Slug = post.Slug,
                Published = post.Published,
                categoriesID = post.PostCategories.Select(c => c.CategoryID).ToArray()
            };
            return View(postEdit);
        }

        // POST: Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Description,Slug,Content,Published,categoriesID")] CreatePostModel post)
        {
            var cates = await _context._categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(cates, "Id", "Title");
            if (id != post.PostId)
            {
                return NotFound();
            }

            if (post.Slug == null)
            {
                App.Utilities.AppUtilities.GenerateSlug(post.Title, true);
            }
            //kiểm tra slug trùng k
            if (await _context.Post.AnyAsync(slug => slug.Slug == post.Slug && post.PostId != id))
            {
                ModelState.AddModelError(string.Empty, "Nhập trùng url cho bài viết");
            };
            if (ModelState.IsValid)
            {
                try
                {
                    var postUpdate = await _context.Post.Include(p => p.PostCategories).FirstOrDefaultAsync(p => p.PostId == id);
                    if (postUpdate == null) return NotFound();
                    postUpdate.Title = post.Title;
                    postUpdate.Content = post.Content;
                    postUpdate.Description = post.Description;
                    postUpdate.Slug = post.Slug;
                    postUpdate.Published = post.Published;
                    postUpdate.DateUpdated = DateTime.Now;
                    if (post.categoriesID == null)
                    {
                        post.categoriesID = new int[] { };
                    }
                    var oldCates = postUpdate.PostCategories.Select(p => p.CategoryID).ToArray();
                    var newCates = post.categoriesID;
                    var revCates = from p in postUpdate.PostCategories
                                   where (!newCates.Contains(p.CategoryID))
                                   select p;
                    var addCates = from c in newCates
                                   where !(oldCates.Contains(c))
                                   select c;
                    _context.PostCategory.RemoveRange(revCates);
                    foreach (var item in addCates)
                    {
                        _context.PostCategory.Add(new PostCategory() { CategoryID = item, PostID = id });
                    }
                    _context.Update(postUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
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
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", post.AuthorId);
            return View(post);
        }

        // GET: Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.FindAsync(id);
            if (post == null) return NotFound();
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            StatusMessage = "Đã xóa thành công!";

            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.PostId == id);
        }
    }
}
