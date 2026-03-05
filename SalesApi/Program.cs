using Microsoft.EntityFrameworkCore;
using SalesApi.Data;
using SalesApi.Resolvers;
using SalesApi.Repositories;
using SalesApi.Rules;
using SalesApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddDbContext<SalesDbContext>(options =>
    options.UseInMemoryDatabase("SalesDb"));

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<ISellService, SellService>();
builder.Services.AddScoped<ISellProductRule, SellProductRule>();
builder.Services.AddScoped<ISellRuleResolver, SellRuleResolver>();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SalesDbContext>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
