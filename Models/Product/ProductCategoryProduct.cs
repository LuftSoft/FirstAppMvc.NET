using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NETMVC.Models.Product
{
    [Table("ProductCategoryProduct")]
    public class ProductCategoryProduct
    {
        public int ProductID { set; get; }
        public int CategoryID { set; get; }
        [ForeignKey("ProductID")]
        public ProductModel Product { set; get; }
        [ForeignKey("CategoryID")]
        public CategoryProduct Category { set; get; }
    }
}