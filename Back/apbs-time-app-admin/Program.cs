using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Shared.Context;
using Shared.Services;
using apbs_time_app_admin.Services.TenantService;
using Shared.Extensions;
using Serilog;
using apbs_time_app_admin.Security;
using apbs_time_app_admin.Services.Security;
using Shared.Models;
using Shared.Models.Mailing;
using Shared.Services.Mailing;
using Shared.Services.Seeds;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();


builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options
    .UseLazyLoadingProxies()
    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<TenantDbContext>(
    options => options
    .UseLazyLoadingProxies()
    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<IMailService, SmtpMailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICurrentTenantService, CurrentTenantService>();
builder.Services.AddTransient<ITenantService, TenantService>();
builder.Services.AddTransient<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddMigrateTenantDatabase(builder.Configuration);
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



//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = builder.Configuration["Jwt:Issuer"],
//            ValidAudience = builder.Configuration["Jwt:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
//            RoleClaimType = "role"
//        };
//        options.Events = new JwtBearerEvents
//        {
//            OnAuthenticationFailed = context =>
//            {
//                Console.WriteLine("Authentication failed: " + context.Exception.Message);
//                return Task.CompletedTask;
//            },
//            OnTokenValidated = context =>
//            {
//                Console.WriteLine("Token validated successfully.");
//                return Task.CompletedTask;
//            }
//        };
//    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.WithOrigins("*")
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    try
//    {
//        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//        dbContext.Database.Migrate();
//    }
//    catch { }
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
