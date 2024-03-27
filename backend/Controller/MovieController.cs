using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Movie;

public class CreateMovieDto
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";

}

public class MovieDto
{

    public Guid Id { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    public Boolean Seen { get; set; }

    public MovieDto(Movie movie)
    {
        if (movie != null)
        {
            this.Id = movie.MovieId;
            this.Title = movie.Title;
            this.Description = movie.Description;
            this.Seen = movie.Seen;
        }
        else
        {
            this.Title = "ERROR";
            this.Description = "ERROR";
        }

    }
}

[ApiController]
[Route("api/movie")]
public class MovieController : ControllerBase
{
    MovieService movieService;
    ApplicationContext context;

    public MovieController(MovieService movieService, ApplicationContext context)
    {
        this.movieService = movieService;
        this.context = context;
    }

    [HttpPost]
    [Authorize("create_movie")]
    public IActionResult CreateMovie([FromBody] CreateMovieDto dto)
    {
        try
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Movie movie = movieService.CreateMovie(dto.Title, dto.Description, id);
            return Ok(new MovieDto(movie));
        }
        catch (DuplicateNameException)
        {
            return Conflict($"A course with the name '{dto.Title}' already exists.");
        }
        catch (ArgumentException)
        {
            return BadRequest($"'Name' and 'Description' must not be null or empty.");
        }
    }



    [HttpGet("movies")]
    [Authorize("get_movies")]
    public List<MovieDto> GetAllMovies()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            throw new ArgumentNullException("Hittade ej användaren");
        }
        return movieService.GetAll(userId).Select(movie => new MovieDto(movie)).ToList();
    }

    [HttpDelete("movie/{title}")]
    [Authorize("remove_movie")]
    public IActionResult RemoveMovie(string title)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Movie? movie = movieService.RemoveMovie(title, userId);
        if (movie == null)
        {
            return NotFound();
        }

        /* MovieDto output = new MovieDto(movie); */
        return Ok("Deleted " + title);
    }

    [HttpPut("movie/{title}")]
    [Authorize("update_movie")]
    public IActionResult UpdateMovie(string title, [FromQuery] bool completed)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return NotFound();
        }

        Movie? movie = movieService.UpdateMovie(title, completed, userId);
        if (movie == null)
        {
            return NotFound();
        }

        MovieDto output = new MovieDto(movie);
        return Ok(output);
    }


    [HttpGet("movie/{title}")]
    [Authorize("detail_movie")]
    public IActionResult DetailMovie(string title)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Movie? movie = movieService.DetailMovie(title, userId);
        if (movie == null)
        {
            return NotFound();
        }

        MovieDto output = new MovieDto(movie);
        return Ok(output.Description);
    }


}
