using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Repository.Repo;
using NicheWebErpAPI.Services.IServ;
using NicheWebErpAPI.Services.Serv;

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

app.UseAuthorization();

app.MapControllers();

app.Run();
