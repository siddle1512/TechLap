using Microsoft.EntityFrameworkCore;
using TechLap.API.Data;
using TechLap.API.Mapper.MappingProfiles;
using TechLap.API.Services.Filters;
using TechLap.API.Services.Repositories.IRepositories;
using TechLap.API.Services.Repositories.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(cfg =>
{
    cfg.Filters.Add(typeof(ExceptionFilter));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//sqlServerConnectionString
builder.Services.AddDbContext<TechLapContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(ProductMappingProfile));

//lifetimeDependencyInjection
builder.Services.AddScoped<IProductRepository, ProductRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
