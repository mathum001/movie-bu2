using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Movie;

public class UserDbContext : DbContext
{

}

public class CreateUserDto
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";

    public CreateUserDto(string username, string password)
    {
        this.Username = username;
        this.Password = password;
    }
}

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserDto dto)
    {
        return Ok("DU skapade en användare!");
    }
}


