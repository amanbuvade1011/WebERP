using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI.Models;

namespace NicheWebErpAPI
{
    public class ERPDbContext : DbContext
    {
        public ERPDbContext(DbContextOptions<ERPDbContext> options)
            : base(options)
        {
        }

        public DbSet<Style> Styles => Set<Style>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductLocation> ProductLocations => Set<ProductLocation>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Label> Labels => Set<Label>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Every table in this DB uses a composite key: (CompanyID, EntityID).
            // There are no real FK constraints in the database, so we only map
            // scalar columns here - no navigation properties yet.
            modelBuilder.Entity<Style>(e =>
            {
                e.ToTable("Style");
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<Product>(e =>
            {
                e.ToTable("Product");
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<ProductLocation>(e =>
            {
                e.ToTable("ProductLocation");
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<Category>(e =>
            {
                e.ToTable("Category");
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<Label>(e =>
            {
                e.ToTable("Label");
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });
        }
    }
}
