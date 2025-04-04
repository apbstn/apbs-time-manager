using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Models;

namespace Shared.Services.Seeds;

public class SeedDataHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public SeedDataHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            // Create the user
            var adminUser = new User
            {
                Username = "admin@example.com",
                Email = "admin@example.com",
                Role = Models.Enumerations.RoleEnum.Admin
            };

            await userService.RegisterAsync(adminUser, "123456");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
