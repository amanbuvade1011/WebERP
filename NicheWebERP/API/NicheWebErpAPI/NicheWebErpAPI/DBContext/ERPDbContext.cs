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
        public DbSet<CompanyLocation> CompanyLocations => Set<CompanyLocation>();
        public DbSet<StyleColor> StyleColors => Set<StyleColor>();
        public DbSet<Sizeway> Sizeways => Set<Sizeway>();
        public DbSet<SizewayItem> SizewayItems => Set<SizewayItem>();
        public DbSet<Size> Sizes => Set<Size>();
        public DbSet<Models.Range> Ranges => Set<Models.Range>();
        public DbSet<Season> Seasons => Set<Season>();
        public DbSet<PricePoint> PricePoints => Set<PricePoint>();
        public DbSet<StylePrice> StylePrices => Set<StylePrice>();
        public DbSet<StyleSellLocation> StyleSellLocations => Set<StyleSellLocation>();
        public DbSet<Firm> Firms => Set<Firm>();
        public DbSet<Person> Persons => Set<Person>();
        public DbSet<TradingTerms> TradingTerms => Set<TradingTerms>();

        // New (Sprint 01) - ERP application users. Deliberately NOT the legacy composite
        // (CompanyID, EntityID)/no-FK pattern - see OnModelCreating below and
        // docs/ai-plan/01-database-map.md for why.
        public DbSet<ErpRole> ErpRoles => Set<ErpRole>();
        public DbSet<ErpUser> ErpUsers => Set<ErpUser>();

        // New (Sprint 05) - Sales Orders live on the generic legacy TransactionBase/
        // TransactionLine/TransactionQuantity tables, discriminated by EntityClassName. See
        // docs/ai-plan/01-database-map.md.
        public DbSet<TransactionBase> TransactionBases => Set<TransactionBase>();
        public DbSet<TransactionLine> TransactionLines => Set<TransactionLine>();
        public DbSet<TransactionQuantity> TransactionQuantities => Set<TransactionQuantity>();

        // New (Sprint 06) - Invoicing & Payments. PaymentMethod is a plain lookup table;
        // FinancialAllocation links a Payment-class TransactionBase to the Invoice-class
        // TransactionBase it's paying off. See docs/ai-plan/01-database-map.md.
        public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
        public DbSet<FinancialAllocation> FinancialAllocations => Set<FinancialAllocation>();

        // New (Sprint 07) - Promotions & Freight.
        public DbSet<Promotion> Promotions => Set<Promotion>();
        public DbSet<CouponPerson> CouponPersons => Set<CouponPerson>();
        public DbSet<FreightCalculator> FreightCalculators => Set<FreightCalculator>();
        public DbSet<FreightItem> FreightItems => Set<FreightItem>();
        public DbSet<TransactionDiscount> TransactionDiscounts => Set<TransactionDiscount>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Every table in this DB uses a composite key: (CompanyID, EntityID).
            // There are no real FK constraints in the database, so we only map
            // scalar columns here - no navigation properties yet.
            //
            // CRITICAL: every one of these 96 legacy tables has AFTER INSERT/UPDATE/DELETE
            // triggers (258 triggers total across the schema, confirmed via sys.triggers on
            // 2026-07-19 - standard audit-log triggers, 3 per table). SQL Server refuses an
            // OUTPUT clause on a DML statement against a table with enabled triggers unless it's
            // OUTPUT ... INTO a table variable, and EF Core 7+ uses a plain OUTPUT clause by
            // default to read back generated/computed values on every insert/update. Without
            // UseSqlOutputClause(false), SaveChanges on ANY of these tables throws
            // DbUpdateException ("target table ... cannot have any enabled triggers if the
            // statement contains an OUTPUT clause"). This bit CompanyLocation the moment a real
            // write was attempted against it - every future entity mapped onto one of the
            // pre-existing 96 tables (Sales, Manufacturing's reused Style/Sizeway, Finance, etc.)
            // MUST repeat `tb.UseSqlOutputClause(false)` below or its first write will 500.
            // New tables we create ourselves (ErpUser, ErpRole, and any future Manufacturing
            // tables) do NOT have triggers and don't need this.
            modelBuilder.Entity<Style>(e =>
            {
                e.ToTable("Style", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<Product>(e =>
            {
                e.ToTable("Product", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<ProductLocation>(e =>
            {
                e.ToTable("ProductLocation", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<Category>(e =>
            {
                e.ToTable("Category", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<Label>(e =>
            {
                e.ToTable("Label", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<CompanyLocation>(e =>
            {
                e.ToTable("CompanyLocation", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<StyleColor>(e =>
            {
                e.ToTable("StyleColor", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<Sizeway>(e =>
            {
                e.ToTable("Sizeway", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<SizewayItem>(e =>
            {
                e.ToTable("SizewayItem", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<Size>(e =>
            {
                e.ToTable("Size", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<Models.Range>(e =>
            {
                e.ToTable("Range", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<Season>(e =>
            {
                e.ToTable("Season", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<PricePoint>(e =>
            {
                e.ToTable("PricePoint", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<StylePrice>(e =>
            {
                e.ToTable("StylePrice", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<StyleSellLocation>(e =>
            {
                e.ToTable("StyleSellLocation", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<Firm>(e =>
            {
                e.ToTable("Firm", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<Person>(e =>
            {
                e.ToTable("Person", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
                // PersonNo is a real IDENTITY column in the DB (not discoverable from
                // INFORMATION_SCHEMA.COLUMNS alone - only surfaced by actually trying an
                // insert, confirmed live in Sprint 04). Without this, EF tries to explicitly
                // insert into it and SQL Server rejects it ("IDENTITY_INSERT is set to OFF").
                e.Property(x => x.PersonNo).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<TradingTerms>(e =>
            {
                e.ToTable("TradingTerms", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            // New (Sprint 01) tables - deliberate exception to the composite-key/no-FK
            // convention above. Single int identity PK, real FK constraints. See
            // docs/ai-plan/01-database-map.md for the rationale.
            modelBuilder.Entity<ErpRole>(e =>
            {
                e.ToTable("ErpRole");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.HasIndex(x => x.Name).IsUnique();
            });

            modelBuilder.Entity<ErpUser>(e =>
            {
                e.ToTable("ErpUser");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.HasIndex(x => x.Username).IsUnique();
                e.HasIndex(x => x.Email).IsUnique();
                e.HasIndex(x => x.LegacyPersonId).IsUnique();

                e.HasOne(x => x.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.UpdatedByUser)
                    .WithMany()
                    .HasForeignKey(x => x.UpdatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TransactionBase>(e =>
            {
                e.ToTable("TransactionBase", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<TransactionLine>(e =>
            {
                e.ToTable("TransactionLine", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<TransactionQuantity>(e =>
            {
                e.ToTable("TransactionQuantity", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<PaymentMethod>(e =>
            {
                e.ToTable("PaymentMethod", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<FinancialAllocation>(e =>
            {
                e.ToTable("FinancialAllocation", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<Promotion>(e =>
            {
                e.ToTable("Promotion", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<CouponPerson>(e =>
            {
                e.ToTable("CouponPerson", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<FreightCalculator>(e =>
            {
                e.ToTable("FreightCalculator", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<FreightItem>(e =>
            {
                e.ToTable("FreightItem", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });

            modelBuilder.Entity<TransactionDiscount>(e =>
            {
                e.ToTable("TransactionDiscount", tb => tb.UseSqlOutputClause(false));
                e.HasKey(x => new { x.CompanyID, x.EntityID });
            });
        }
    }
}
