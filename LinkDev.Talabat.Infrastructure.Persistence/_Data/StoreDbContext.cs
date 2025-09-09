using LinkDev.Talabat.Core.Domain.Entities.Orders;
using LinkDev.Talabat.Core.Domain.Entities.Products;
using LinkDev.Talabat.Infrastructure.Persistence._Common;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace LinkDev.Talabat.Infrastructure.Persistence._Data
{
    public class StoreDbContext :DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBrand> Brands { get; set; }
        public DbSet<ProductCategory> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssemblyInformation).Assembly,
                type=>type.GetCustomAttribute<DbContextTypeAttribute>()?.DbContextType==typeof(StoreDbContext));

        }
    

    }
}
