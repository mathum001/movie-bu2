using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Backend;

public class User : IdentityUser
{
    public List<Movie> Movies { get; set; }

    public User() { }
}