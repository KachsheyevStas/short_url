using short_url.Models;
using Microsoft.AspNetCore.Mvc;
using short_url.Models.Users;
using short_url.Services;
using short_url.Helpers;

namespace short_url.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("authenticate")]
    public IActionResult Authenticate(AuthenticateRequest model)
    {
        var response = _userService.Authenticate(model);

        if (response == null)
            return BadRequest(new { message = "Username or password is incorrect" });

        return Ok(response);
    }

    [HttpPost("registration")]
    public IActionResult Registration(RegisterRequest model)
    {
        try
        {
            _userService.Register(model);
            return Ok(new { message = "User created" });
        }
        catch (AppException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var users = _userService.GetAll();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var user = _userService.GetById(id);
        return Ok(user);
    }

    

    [HttpPut("{id}")]
    public IActionResult Update(Guid id, UpdateRequest model)
    {
        _userService.Update(id, model);
        return Ok(new { message = "User updated" });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        _userService.Delete(id);
        return Ok(new { message = "User deleted" });
    }
}