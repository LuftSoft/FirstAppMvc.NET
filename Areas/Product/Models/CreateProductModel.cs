using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ASP.NETMVC.Models.Blog;

namespace ASP.NETMVC.Areas.Product.Models
{
    public class CreateProductModel : ASP.NETMVC.Models.Product.ProductModel
    {
        [Display(Name = "Chuyên mục")]
        public int[] categoriesID { set; get; }
    }
}