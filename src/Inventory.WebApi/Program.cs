using Inventory.Application;
using Inventory.Infrastructure;
using Inventory.WebApi;
using Inventory.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddAplication()
    .AddInfrastructure(builder.Configuration, builder.Environment)
    .AddPresentation();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUi();
    app.ApplyMigrations();
}

app.UseRouting();

app.UseHealthChecks();

app.UseRequestCorrelationId();

app.UseRequestContextLogging();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
