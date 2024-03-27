using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend;

public class ApplicationContext : IdentityDbContext<User>
{
    public DbSet<Movie> Movies { get; set; }



    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options) { }
}

public interface IMovieRepository
{
    Movie SaveMovie(Movie movie);
    Movie RemoveMovie(Movie movie);
    Movie UpdateMovie(Movie movie);
}

public class MovieRepository : IMovieRepository
{
    ApplicationContext context;

    public MovieRepository(ApplicationContext context)
    {
        this.context = context;
    }

    public Movie SaveMovie(Movie movie)
    {
        if (movie == null)
        {
            throw new ArgumentNullException(nameof(movie));
        }
        context.Movies.Add(movie);
        movie.user.Movies.Add(movie); // kan nog ta bort denna rad
        context.SaveChanges();
        return movie;
    }

    public Movie RemoveMovie(Movie movie)
    {
        if (movie == null)
        {
            throw new ArgumentNullException(nameof(movie));
        }
        context.Movies.Remove(movie);
        movie.user.Movies.Remove(movie);// kan nog ta bort denna rad

        context.SaveChanges();
        return movie;
    }

    public Movie UpdateMovie(Movie movie)
    {
        if (movie == null)
        {
            throw new ArgumentNullException(nameof(movie));
        }

        context.Movies.Update(movie);
        /* movie.user.Movies.Update(movie); */
        context.SaveChanges();

        return movie;
    }
}