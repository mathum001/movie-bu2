using System.Data;

namespace Movie;

public class MovieService
{
    ApplicationContext context;
    IMovieRepository movieRepository;

    public MovieService(ApplicationContext context, IMovieRepository movieRepository)
    {
        this.context = context;
        this.movieRepository = movieRepository;
    }

    public Movie CreateMovie(string title, string description, string id)
    {
        int existingCount = context.Movies.Where(existing => existing.Title == title).Count();
        if (existingCount > 0)
        {
            throw new DuplicateNameException();
        }

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException();
        }
        var user = context.Users.Find(id);
        if (user == null)
        {
            throw new ArgumentException("No such user");
        }

        return movieRepository.SaveMovie(new Movie(title, description, user));

    }

    public Movie RemoveMovie(string title, string id) // userid är det!
    {
        User? user = context.Users.Find(id);
        if (user == null)
        {
            throw new ArgumentException("Can't find user :/ (removemovie)");
        }
        List<Movie> movies = context.Movies.Where(movie => movie.user.Id == user.Id && movie.Title == title).ToList();

        int existingCount = context.Movies.Where(existing => existing.Title == title).Count();
        if (movies.Count == 0)
        {
            return null;
        }
        Movie movie = movies[0];

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Fel på titel");
        }

        return movieRepository.RemoveMovie(movie);

    }



    public List<Movie> GetAll(string userId)
    {
        User? user = context.Users.Find(userId);
        if (user == null)
        {
            throw new ArgumentNullException("Fel på anända");
        }
        return context.Movies.Where(movie => movie.user.Id == userId).ToList();
        /* return user.Movies.ToList(); */
    }

    public Movie DetailMovie(string title, string userId)
    {
        User? user = context.Users.Find(userId);
        if (user == null)
        {
            throw new ArgumentNullException("Fel på anända");
        }
        List<Movie> movies = context
           .Movies.Where(movie => movie.Title == title && movie.user.Id == userId)
           .ToList();
        if (movies.Count == 0)
        {
            return null;
        }

        Movie movie = movies[0];
        return movie;
    }

    public Movie UpdateMovie(string title, bool completed, string userId)
    {
        User user = context.Users.Find(userId);
        if (user == null)
        {
            throw new ArgumentNullException("Fel på användare");
        }
        /* Movie movieUpdate = context.Movies.FirstOrDefault(movie => movie.user.Id == userId && movie.Title == title);
        if (movieUpdate == null)
        {

            throw new ArgumentNullException("Null");
        } */
        List<Movie> movies = context
           .Movies.Where(movie => movie.Title == title && movie.user.Id == userId)
           .ToList();
        if (movies.Count == 0)
        {
            return null;
        }

        Movie movie = movies[0];

        movie.Seen = completed;
        context.Movies.Update(movie);
        context.SaveChanges();
        /* return movieRepository.UpdateMovie(movie); */
        return movie;


    }
}