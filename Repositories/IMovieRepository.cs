using Movies.Api.Models;

namespace Movies.Api.Repositories;

public interface IMovieRepository
{
    Task<Movie> GetMovieAsync(string id);
    Task<IEnumerable<Movie>> GetMoviesAsync();
    Task AddMovieAsync(Movie movie);
    Task UpdateMovieAsync(string id, Movie movie);
    Task DeleteMovieAsync(string id);
    Task<IEnumerable<Movie>> GetMoviesBySearchQueryAsync(string searchQuery);
}