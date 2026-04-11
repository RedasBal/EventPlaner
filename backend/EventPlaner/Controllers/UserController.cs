using EventPlaner.DT0s;
using EventPlaner.Models;
using EventPlaner.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPlaner.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly UserService _users;
    private readonly TokenService _tokens;

    public UserController(UserService users, TokenService tokens)
    {
        _users = users;
        _tokens = tokens;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] UserLoginDto request)
    {
        var password = !string.IsNullOrWhiteSpace(request?.Password)
            ? request.Password
            : request?.PasswordHash ?? string.Empty;

        if (request == null ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(password))
        {
            return BadRequest("Neteisingi duomenys");
        }

        User? user = _users.Login(request.Email, password);
        if (user == null)
        {
            return Unauthorized("Neteisingi duomenys");
        }

        var token = _tokens.CreateToken(user);
        return Ok(new AuthResponseDto
        {
            Token = token,
            User = UserMapper.Map(user)
        });
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public IActionResult Register([FromBody] UserRegisterDto request)
    {
        var password = !string.IsNullOrWhiteSpace(request?.Password)
            ? request.Password
            : request?.PasswordHash ?? string.Empty;

        if (request == null ||
            string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(password))
        {
            return BadRequest("Neteisingi duomenys");
        }

        User? user = _users.Register(request.Username, request.Email, password);
        if (user == null)
        {
            return Unauthorized("Vartotojas toks jau yra");

        }

        var token = _tokens.CreateToken(user);
        return Ok(new AuthResponseDto
        {
            Token = token,
            User = UserMapper.Map(user)
        });
    }

}
