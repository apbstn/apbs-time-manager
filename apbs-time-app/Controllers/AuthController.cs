using apbs_time_app.Services.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;
using System.Security.Claims;

namespace Apbs_Time_App.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtTokenGenerator _jwtGen;

    public AuthController(IUserService userService, IJwtTokenGenerator jwtGen)
    {
        _userService = userService;
        _jwtGen = jwtGen;
    }

    [AllowAnonymous]
    [HttpGet("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        var user = await _userService.AuthenticateAsync(loginModel.Email, loginModel.Password);

        var authToken = _jwtGen.GenerateAuthToken(user);

        var accessToken = await _jwtGen.GenerateAccessToken(user);

        return Ok(new
        {
            authToken = authToken.Result,
            accessToken
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


        var access = await _userService.CheckAccess(userId, tenantId);
        if (!access)
            return Unauthorized();

        var user = await _userService.GetUser(userEmail);

        var accessToken = _jwtGen.GenerateAccessToken(user, tenantId, Request.Headers["Authorization"].ToString());

        return Ok(new
        {
            accessToken = accessToken.Result
        });

    }
}

public class LoginModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}