using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NicheWebErpAPI;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Repository.Repo;
using NicheWebErpAPI.Services.IServ;
using NicheWebErpAPI.Services.Serv;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//DB conection
builder.Services.AddDbContext<ERPDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Generic EF Core service used by repositories to talk to the DB
builder.Services.AddScoped(typeof(IEfCoreService<>), typeof(EfCoreService<>));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

// Sprint 01 - Foundation (Auth, ERP staff users, minimal company lookup)
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IErpUserRepository, ErpUserRepository>();
builder.Services.AddScoped<IErpUserService, ErpUserService>();
builder.Services.AddScoped<IErpRoleRepository, ErpRoleRepository>();
builder.Services.AddScoped<IErpRoleService, ErpRoleService>();
builder.Services.AddScoped<ICompanyLocationRepository, CompanyLocationRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ILocationService, LocationService>();

// Sprint 02 - Product Master Data
builder.Services.AddScoped<IStyleRepository, StyleRepository>();
builder.Services.AddScoped<IStyleService, StyleService>();
builder.Services.AddScoped<ISizewayRepository, SizewayRepository>();
builder.Services.AddScoped<ISizewayService, SizewayService>();
builder.Services.AddScoped<ISizeRepository, SizeRepository>();
builder.Services.AddScoped<ISizeService, SizeService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ILabelRepository, LabelRepository>();
builder.Services.AddScoped<ILabelService, LabelService>();
builder.Services.AddScoped<IRangeRepository, RangeRepository>();
builder.Services.AddScoped<IRangeService, RangeService>();
builder.Services.AddScoped<ISeasonRepository, SeasonRepository>();
builder.Services.AddScoped<ISeasonService, SeasonService>();

// Sprint 03 - Product Pricing & Stock
builder.Services.AddScoped<IStylePricingRepository, StylePricingRepository>();
builder.Services.AddScoped<IStylePricingService, StylePricingService>();
builder.Services.AddScoped<IProductGenerationRepository, ProductGenerationRepository>();
builder.Services.AddScoped<IProductGenerationService, ProductGenerationService>();

// Sprint 04 - Sales Customers
builder.Services.AddScoped<IFirmRepository, FirmRepository>();
builder.Services.AddScoped<IFirmService, FirmService>();
builder.Services.AddScoped<IRetailCustomerRepository, RetailCustomerRepository>();
builder.Services.AddScoped<IRetailCustomerService, RetailCustomerService>();
builder.Services.AddScoped<ITradingTermsRepository, TradingTermsRepository>();
builder.Services.AddScoped<ITradingTermsService, TradingTermsService>();

// Sprint 05 - Sales Orders
builder.Services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
builder.Services.AddScoped<ISalesOrderService, SalesOrderService>();

// Sprint 06 - Invoicing & Payments
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();

// Sprint 07 - Promotions & Freight
builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IFreightRepository, FreightRepository>();
builder.Services.AddScoped<IFreightService, FreightService>();

var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

const string AllowAllCorsPolicy = "AllowAll";
builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowAllCorsPolicy, policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors(AllowAllCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await SeedInitialAdminAsync(app);

app.Run();

// Sprint 01 one-time bootstrap: creates an "Admin" ErpRole and a default admin ErpUser if
// none exist yet, since there is no other way to create the very first login. Dev-only
// password - change it immediately via ResetPassword after first login in any real environment.
static async Task SeedInitialAdminAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ERPDbContext>();
    var companyRepo = scope.ServiceProvider.GetRequiredService<ICompanyLocationRepository>();

    if (!await db.ErpUsers.AnyAsync())
    {
        var adminRole = await db.ErpRoles.FirstOrDefaultAsync(r => r.Name == "Admin");
        if (adminRole is null)
        {
            adminRole = new ErpRole
            {
                Name = "Admin",
                Description = "Full access - seeded by initial startup bootstrap.",
                CreatedAt = DateTime.UtcNow
            };
            db.ErpRoles.Add(adminRole);
            await db.SaveChangesAsync();
        }

        var companyId = await companyRepo.GetMasterCompanyIdAsync();

        var adminUser = new ErpUser
        {
            FirstName = "System",
            LastName = "Admin",
            Username = "admin",
            Email = "admin@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("ChangeMe123!"),
            RoleId = adminRole.Id,
            CompanyId = companyId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            LegacyPersonId = Guid.NewGuid()
        };
        db.ErpUsers.Add(adminUser);
        await db.SaveChangesAsync();

        Console.WriteLine("=========================================================");
        Console.WriteLine("Seeded initial ERP admin user:");
        Console.WriteLine("  Username: admin");
        Console.WriteLine("  Password: ChangeMe123!  <-- CHANGE THIS after first login");
        Console.WriteLine("=========================================================");
    }
}
