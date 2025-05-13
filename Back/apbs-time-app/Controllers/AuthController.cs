using apbs_time_app.Services;
using apbs_time_app.Services.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.UserDtos;
using Shared.Models;
using Shared.Models.Requests;
using Shared.Services;
using System.Security.Claims;

namespace Apbs_Time_App.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtTokenGenerator _jwtGen;
    private readonly ITenantService _tenantService;

    public AuthController(IUserService userService, IJwtTokenGenerator jwtGen, ITenantService tenantService)
    {
        _userService = userService;
        _jwtGen = jwtGen;
        _tenantService = tenantService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        var user = await _userService.AuthenticateAsync(loginModel.Email, loginModel.Password);

        if (!user.Registred)
            return Unauthorized();

        //var authToken = _jwtGen.GenerateAuthToken(user);

        (var accessToken, var role) = await _jwtGen.GenerateAccessToken(user);

        var listTen = await _tenantService.GetListTenantOfUser(user.Id);

        return Ok(new
        {
            //authToken = authToken.Result,
            accessToken,
            user.Id,
            listTen,
            user.Username,
            role
        });
    }
    [Authorize]
    [HttpGet("switch")]
    public async Task<IActionResult> Switch(string tenantId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail))
            return Unauthorized();


        var access = await _userService.CheckAccess(Guid.Parse(userId), Guid.Parse(tenantId));
        if (!access)
            return Unauthorized();

        var user = await _userService.GetUser(userEmail);

        var accessToken = _jwtGen.GenerateAccessToken(user, tenantId, Request.Headers["Authorization"].ToString());

        return Ok(new
        {
            accessToken = accessToken.Result
        });

    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterRequest request)
    {
        var user = await _userService.GetUser(request.Email);
        if (user != null)
            return Unauthorized();


        var newUser = new User
        {
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Username = request.Username,
            Registred = true
        };

        var ss = await _userService.RegisterAsync(newUser, request.Password);

        (var accessToken, var role) = await _jwtGen.GenerateAccessToken(newUser);

        return Ok(new
        {
            accessToken,
            role
        });
    }


}

public class LoginModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}