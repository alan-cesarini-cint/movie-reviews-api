using Microsoft.AspNetCore.Mvc;
using Moq;
using Movies.Api.Controllers;
using Movies.Api.Models;
using Movies.Api.Repositories;

namespace Movies.Api.Tests;

public class MoviesControllerTests
{
    private readonly MoviesController _controller;

    //private readonly Mock<ILogger<MoviesController>> _loggerMock;
    private readonly Mock<IMovieRepository> _movieRepositoryMock;
    private readonly Mock<IReviewRepository> _reviewRepositoryMock;

    public MoviesControllerTests()
    {
        // Mocking repositories
        _movieRepositoryMock = new Mock<IMovieRepository>();
        _reviewRepositoryMock = new Mock<IReviewRepository>();
        //_loggerMock = new Mock<ILogger<MoviesController>>();

        // Injecting mocks into the controller
        _controller =
            new MoviesController(_movieRepositoryMock.Object, _reviewRepositoryMock.Object, null);
    }

    [Fact]
    public Task GetMovieById_ReturnsNotFound_WhenMovieDoesNotExist()
    {
        // Arrange
        var movieId = "non-existing-id";
        _movieRepositoryMock.Setup(repo => repo.GetMovieAsync(movieId))
            .ReturnsAsync((Movie)null); // Return null to simulate not found

        // Act
        var result = _controller.GetMovieById(movieId);

        // Assert
        var actionResult = Assert.IsType<NotFoundResult>(result);

        return Task.FromResult(actionResult);
    }

    [Fact]
    public async Task GetMovieById_ReturnsOk_WhenMovieExists()
    {
        // Arrange
        var movieId = "existing-id";
        var movie = new Movie
            { Id = movieId, Title = "Test Movie", Genre = "Action", Synopsis = "synopsis", Year = 1999 };
        _movieRepositoryMock.Setup(repo => repo.GetMovieAsync(movieId))
            .ReturnsAsync(movie);

        // Act
        var result = _controller.GetMovieById(movieId);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnMovie = Assert.IsType<Movie>(actionResult.Value);
        Assert.Equal(movieId, returnMovie.Id);
    }

    [Fact]
    public async Task GetMovies_ReturnsOk_WhenMoviesExist()
    {
        // Arrange
        var movies = new List<Movie>
        {
            new() { Id = "1", Title = "Movie 1", Genre = "Action", Synopsis = "synopsis", Year = 1999 },
            new() { Id = "2", Title = "Movie 2", Genre = "Drama", Synopsis = "synopsis", Year = 1998 }
        };

        _movieRepositoryMock.Setup(repo => repo.GetMoviesAsync())
            .ReturnsAsync(movies);

        // Act
        var result = _controller.GetMovies();

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnMovies = Assert.IsType<List<Movie>>(actionResult.Value);
        Assert.Equal(2, returnMovies.Count);
    }

    [Fact]
    public async Task AddMovie_ReturnsCreatedAtAction_WhenMovieIsAddedSuccessfully()
    {
        // Arrange
        var newMovie = new Movie { Title = "New Movie", Genre = "Comedy", Synopsis = "synopsis", Year = 1999 };
        _movieRepositoryMock.Setup(repo => repo.AddMovieAsync(newMovie))
            .Returns(Task.CompletedTask);

        // Act
        var result = _controller.AddMovie(newMovie);

        // Assert
        var actionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("GetMovieById", actionResult.ActionName);
    }

    [Fact]
    public async Task DeleteMovie_ReturnsNotFound_WhenMovieDoesNotExist()
    {
        // Arrange
        var movieId = "non-existing-id";
        _movieRepositoryMock.Setup(repo => repo.GetMovieAsync(movieId))
            .ReturnsAsync((Movie)null); // Movie does not exist

        // Act
        var result = _controller.DeleteMovie(movieId);

        // Assert
        var actionResult = Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteMovie_ReturnsNoContent_WhenMovieIsDeleted()
    {
        // Arrange
        var movieId = "existing-id";
        var movie = new Movie
            { Id = movieId, Title = "Test Movie", Genre = "Action", Synopsis = "synopsis", Year = 1999 };
        _movieRepositoryMock.Setup(repo => repo.GetMovieAsync(movieId))
            .ReturnsAsync(movie);
        _movieRepositoryMock.Setup(repo => repo.DeleteMovieAsync(movieId))
            .Returns(Task.CompletedTask);

        // Act
        var result = _controller.DeleteMovie(movieId);

        // Assert
        var actionResult = Assert.IsType<NoContentResult>(result);
    }
}