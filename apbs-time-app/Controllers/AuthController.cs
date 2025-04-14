using apbs_time_app.Services.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;

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

        var accessToken = _jwtGen.GenerateAccessToken(user);

        return Ok(new
        {
            authToken,
            accessToken
        });
    }
}

public class LoginModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}
