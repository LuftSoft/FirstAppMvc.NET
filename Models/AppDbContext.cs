using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP.NETMVC.Models.Blog;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASP.NETMVC.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Category> _categories { set; get; }
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
                entity.HasIndex(c => c.Slug);
            });
        }
    }
}