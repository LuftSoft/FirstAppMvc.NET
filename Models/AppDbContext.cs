using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP.NETMVC.Models.Blog;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ASP.NETMVC.Models.Product;

namespace ASP.NETMVC.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Phương thức khởi tạo này chứa options để kết nối đến MS SQL Server
            // Thực hiện điều này khi Inject trong dịch vụ hệ thống
        }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var item in builder.Model.GetEntityTypes())
            {
                var tableName = item.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    item.SetTableName(tableName.Substring(6));
                }
            }
            //fluence API
            builder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.Slug).IsUnique();
            });
            builder.Entity<PostCategory>(entity =>
            {
                entity.HasKey(c => new { c.PostID, c.CategoryID });
            });
            builder.Entity<Post>(entity =>
            {
                entity.HasIndex(c => c.Slug).IsUnique();
            });

            //fluent API cho cart
            builder.Entity<ProductModel>(entity =>
            {
                entity.HasIndex(c => c.Slug).IsUnique();
            });
            builder.Entity<CategoryProduct>(entity =>
            {
                entity.HasIndex(c => c.Slug).IsUnique();
            });
            builder.Entity<ProductCategoryProduct>(entity =>
            {
                entity.HasKey(c => new { c.ProductID, c.CategoryID });
            });

        }
        //khai báo các  table blog
        public DbSet<Category> _categories { set; get; }
        public DbSet<ASP.NETMVC.Models.Blog.Post> Post { get; set; }
        public DbSet<ASP.NETMVC.Models.Blog.PostCategory> PostCategory { get; set; }

        //khai báo các  table giỏ hàng
        public DbSet<ProductModel> Products { set; get; }
        public DbSet<ASP.NETMVC.Models.Product.CategoryProduct> CategoryProducts { get; set; }
        public DbSet<ASP.NETMVC.Models.Product.ProductCategoryProduct> ProductCategoryProducts { get; set; }
    }
}