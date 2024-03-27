using NUnit.Framework;

namespace Movie.Tests
{
    [TestFixture]
    public class MovieTests
    {
        [Test]
        public void Constructor_WithValidParameters_SetsPropertiesCorrectly()
        {
            // Arrange
            string title = "Test Movie";
            string description = "This is a test movie.";
            var user = new User(); // Assuming User class exists and is instantiated correctly

            // Act
            var movie = new Movie(title, description, user);

            // Assert
            Assert.AreEqual(title, movie.Title);
            Assert.AreEqual(description, movie.Description);
            Assert.AreEqual(user, movie.user);
        }

        [Test]
        public void DefaultConstructor_SetsPropertiesToDefaultValues()
        {
            // Arrange & Act
            var movie = new Movie();

            // Assert
            Assert.AreEqual(default(Guid), movie.MovieId);
            Assert.IsNull(movie.Title);
            Assert.IsNull(movie.Description);
            Assert.IsFalse(movie.Seen);
            Assert.IsNull(movie.user);
        }
    }
}
