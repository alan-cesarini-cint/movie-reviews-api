using Movies.Api.Models;

namespace Movies.Api.Repositories;

public interface IReviewRepository
{
    Task AddReviewAsync(Review review);
    Task<Review> GetReviewAsync(string id);
    Task<IEnumerable<Review>> GetReviewsAsync();
    Task<IEnumerable<Review>> GetReviewsByUserIdAsync(string id);
    Task<IEnumerable<Review>> GetReviewsByMovieIdAsync(string id);
}