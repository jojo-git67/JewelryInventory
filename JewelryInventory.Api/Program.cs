using JewelryInventory.Api.Data;
using JewelryInventory.Api.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<ComponentService>();
builder.Services.AddScoped<ManualAdjustmentService>();
builder.Services.AddSingleton<InventoryLockService>();
builder.Services.AddScoped<ComponentInventoryMovementService>();
builder.Services.AddScoped<ReceiveOrderService>();
builder.Services.AddProblemDetails();


builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<InventoryDbContext>();
    
    await dbContext.Database.MigrateAsync();
}

app.UseExceptionHandler();

// Configure the HTTP request pipeline for DEVELOPMENT only
// Scalar code from:
// https://medium.com/@syeedmdtalha/getting-started-with-scalar-api-documentation-in-asp-net-core-a-beginners-guide-4d19db387369
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    
    app.MapGet("/", () => Results.Redirect("/scalar"));
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
