using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP.NETMVC.Models.Blog;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NETMVC.Components
{
    [ViewComponent]
    public class CategorySidebar : ViewComponent
    {
        public class CategorySidebarData
        {
            public List<Category> Categories { set; get; }
            public int level { set; get; }
            public string categorySlug { set; get; }
        }
        public IViewComponentResult Invoke(CategorySidebarData data)
        {
            return View(data);
        }
    }
}