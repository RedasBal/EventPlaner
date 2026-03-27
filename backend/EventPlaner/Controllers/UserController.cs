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
        User? user = _users.Login(request.Email, request.PasswordHash);
        if (user == null)
        {
            return Unauthorized("Neteisingi duomenys");
        }
        return Ok(user);
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] UserRegisterDto request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.PasswordHash))
        {
            return BadRequest("Neteisingi duomenys");
        }

        User? user = _users.Register(request.Username, request.Email, request.PasswordHash);
        if (user == null)
        {
            return Unauthorized("Vartotojas toks jau yra");

        }
        return Ok(user);
    }

}
