using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP.NETMVC.Models;

namespace ASP.NETMVC.Services
{
    public class ProductService : List<ProductModel>
    {
        public ProductService()
        {
            this.AddRange(new ProductModel[]{
                new ProductModel(){ID=1,Name="Iphone",Price = 1000},
                new ProductModel(){ID=2,Name="Samsung",Price = 960},
                new ProductModel(){ID=3,Name="Xiaomi",Price = 499}
            });
        }
    }
}