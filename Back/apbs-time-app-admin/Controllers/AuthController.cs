using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Shared.Services;
using apbs_time_app_admin.Services.Security;

namespace apbs_time_app_admin.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthController(IUserService userService,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userService = userService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userService.AuthenticateAsync(model.Email, model.Password);

        if (user == null)
            return Unauthorized();

        if (!user.IsAdmin)
            return Unauthorized();

        var token = _jwtTokenGenerator.Generate(user);
        return Ok(new { Token = token});
    }


    //[HttpPost("register")]
    //public async Task<IActionResult> Register([FromBody] RegisterModel model)
    //{
    //    var userRegister = await _userService.RegisterAsync(new User
    //    {
    //        Username = model.Username,
    //        Email = model.Email
    //    }, model.Password);

    //    if (userRegister)
    //    {
    //        return Ok();
    //    }
    //    else
    //    {
    //        return BadRequest();
    //    }
    //}

    //private string GenerateJwtToken(User user)
    //{
    //    var tokenHandler = new JwtSecurityTokenHandler();
    //    var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
    //    var tokenDescriptor = new SecurityTokenDescriptor
    //    {
    //        Subject = new ClaimsIdentity(new[]
    //        {
    //            new Claim(ClaimTypes.Name, user.Username),
    //            new Claim(ClaimTypes.Email, user.Email),
    //            new Claim(ClaimTypes.Role, user.Role.ToString())
    //        }),
    //        Expires = DateTime.UtcNow.AddDays(7),
    //        Issuer = _configuration["Jwt:Issuer"],
    //        Audience = _configuration["Jwt:Audience"],
    //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    //    };
    //    var token = tokenHandler.CreateToken(tokenDescriptor);
    //    return tokenHandler.WriteToken(token);
    //}
}

public class LoginModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class RegisterModel
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
