using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Movie;

public class User : IdentityUser
{
    public List<Movie> Movies { get; set; }

    public User() { }
}