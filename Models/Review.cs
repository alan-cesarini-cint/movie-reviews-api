using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Movies.Api.Models;

public class Review : IReview
{
    [JsonPropertyName("id")] public string Id { get; set; } = Guid.NewGuid().ToString();
    [JsonPropertyName("movie_id")] public string MovieId { get; set; }
    [JsonPropertyName("user_id")] public string UserId { get; set; }
    [Required(ErrorMessage = "The Rating field is required.")]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    [JsonPropertyName("rating")] public required int Rating { get; set; }
    [Required(ErrorMessage = "The Comment field is required.")]
    [JsonPropertyName("comment")] public required string Comment { get; set; } = string.Empty;
}