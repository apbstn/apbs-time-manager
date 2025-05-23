using apbs_time_app.Services;
using apbs_time_app.Services.Security;
using Apbs_Time_App.Client.Shared.Services.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Shared.Context;
using Shared.Extensions;
using Shared.Middleware;
using Shared.Models.Mailing;
using Shared.Services;
using Shared.Services.Mailing;
using Shared.Services.Seeds;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<TenantDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<IMailService, SmtpMailService>();
builder.Services.AddScoped<ICurrentTenantService, CurrentTenantService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddTransient<IEncryptionService, EncryptionService>();
builder.Services.AddTransient<ITenantService, TenantService>();
builder.Services.AddTransient<IInvitationService, InvitationService>();
builder.Services.AddTransient<IEncryption, Encryption>();

await builder.Services.AddMigrateTenantDatabase(builder.Configuration);
builder.Services.AddHostedService<SeedDataHostedService>();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = JwtExtensions.GetTokenValidationParameters();
        options.SaveToken = true;
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = (context) =>
            {
                var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(authHeader))
                {
                    if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Token = authHeader.Substring("Bearer ".Length).Trim();
                        return Task.CompletedTask;
                    }
                }

                return Task.CompletedTask;
            }
        };
    }
    );



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

//using (var scope = app.Services.CreateScope())
//{
//    try
//    {
//        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//        dbContext.Database.Migrate();
//    }
//    catch { }
//}

app.UseAuthorization();

app.UseMiddleware<TenantResolver>();

app.MapControllers();

app.Run();
