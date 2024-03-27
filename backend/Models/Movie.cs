namespace Backend;

public class Movie
{
    public Guid MovieId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Boolean Seen { get; set; }

    public User user { get; set; }

    public Movie() { }

    public Movie(string title, string description, User user)
    {
        this.Title = title;
        this.Description = description;
        this.user = user;
    }
}