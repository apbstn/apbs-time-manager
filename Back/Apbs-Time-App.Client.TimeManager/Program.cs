using Apbs_Time_App.Client.Shared.Services.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Shared.Context;
using Shared.Extensions;
using Shared.Middleware;
using Shared.Services;
using Shared.Services.Mailing;
using Shared.Services.Planner;
using Shared.Services.Seeds;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<TenantDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ITeamService, TeamService>();


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<IMailService, SmtpMailService>();
builder.Services.AddScoped<ICurrentTenantService, CurrentTenantService>();
builder.Services.AddTransient<IEncryptionService, EncryptionService>();
builder.Services.AddTransient<IFixedPlannerService, FixedPlannerService>();

builder.Services.AddTransient<ITimeLogService, TimeLogService>();
builder.Services.AddSingleton<IExxception, Exxception>();

builder.Services.AddTransient<ILeaveRequestService, LeaveRequestService>();
builder.Services.AddTransient<ILeaveRequestRepository, LeaveRequestRepository>();

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

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.UseMiddleware<TenantResolver>();
app.MapControllers();

app.UseCors("AllowAll");
//using (var scope = app.Services.CreateScope())
//{
//    try
//    {
//        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//        dbContext.Database.Migrate();
//    }
//    catch { }
//}


app.Run();