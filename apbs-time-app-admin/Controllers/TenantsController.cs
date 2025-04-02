using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using apbs_time_app_admin.Services.TenantService;
using apbs_time_app_admin.Services.TenantService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Shared.Services;
using Shared.DTOs;
using System.Text;

namespace apbs_time_app_admin.Controllers;

[Route("api/tenant")]
[ApiController]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;
    private readonly IUserService _userService;
    public TenantsController(ITenantService tenantService, IUserService userService)
    {
        _tenantService = tenantService;
        _userService = userService;
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Post(CreateTenantRequest request)
    {
        var user = new UserDto
        {
            Email = request.Email,
            Username = request.Username,
            PhoneNumber = request.PhoneNumber,
            Password = generatePass(16)
        };
        var resultUser = await _userService.SetUser(user);
        var result = await _tenantService.CreateTenant(request, resultUser);
        return Ok(new {result, user});
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _tenantService.GetAll();
        return Ok(result);
    }

    private string generatePass(int length)
    {
        const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        StringBuilder res = new StringBuilder();
        Random rnd = new Random();
        while (0 < length--)
        {
            res.Append(valid[rnd.Next(valid.Length)]);
        }
        return res.ToString();
    }
}
