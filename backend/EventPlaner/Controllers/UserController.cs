using EventPlaner.DT0s;
using EventPlaner.Models;
using EventPlaner.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventPlaner.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly UserService _users;

    public UserController(UserService users)
    {
        _users = users;
    }

    [HttpPost("login")]
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
        return Ok(UserMapper.Map(user));
    }

    [HttpPost("register")]
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
        return Ok(UserMapper.Map(user));
    }

}
