namespace Movies.Api.Models;

public interface IReview
{
    string Id { get; set; }
    string MovieId { get; set; }
    string UserId { get; set; }
    int Rating { get; set; }
    string Comment { get; set; }
}