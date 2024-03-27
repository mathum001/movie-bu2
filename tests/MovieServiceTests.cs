/* using Xunit;
using Microsoft.EntityFrameworkCore;
using Moq;
using Backend;

namespace tests
{
    public class MovieServiceTests
    {
        [Fact]
        public void CreateMovie_Should_Return_New_Movie()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new ApplicationContext(options))
            {
                // Förbered en användare i databasen
                var user = new User { Id = "001" };
                context.Users.Add(user);
                context.SaveChanges();

                // Skapa en mockning av IMovieRepository
                var mockMovieRepository = new Mock<IMovieRepository>();

                // Konfigurera mocken för att returnera en Movie-objekt när SaveMovie-metoden anropas
                mockMovieRepository.Setup(repo => repo.SaveMovie(It.IsAny<Movie>())).Returns((Movie movie) => movie);

                // Skapa en instans av MovieService med mockade IMovieRepository
                var movieService = new MovieService(context, mockMovieRepository.Object);

                var title = "Test Movie";
                var description = "This is a test movie";
                var id = "001";

                // Act
                var result = movieService.CreateMovie(title, description, id);

                // Assert
                Xunit.Assert.NotNull(result);
                Xunit.Assert.Equal(title, result.Title);
                Xunit.Assert.Equal(description, result.Description);
                Xunit.Assert.False(result.Seen); // Assuming default value for Seen property is false
            }
        }
    }
} */