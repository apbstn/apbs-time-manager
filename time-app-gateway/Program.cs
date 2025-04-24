using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

//builder.Configuration
//    .SetBasePath(Directory.GetCurrentDirectory())
//    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
//    .AddEnvironmentVariables();

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

// Add services to the container.

//builder.Services.AddMigrateTenantDatabase(builder.Configuration);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Log to the console
    .CreateLogger();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseOcelot().Wait();

app.MapControllers();

app.Run();
