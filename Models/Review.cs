using System.Text.Json.Serialization;

namespace Movies.Api.Models;

public class Review : IReview
{
    [JsonPropertyName("id")] public string Id { get; set; } = Guid.NewGuid().ToString();
    [JsonPropertyName("movie_id")] public string MovieId { get; set; }
    [JsonPropertyName("user_id")] public string UserId { get; set; }
    [JsonPropertyName("rating")] public required int Rating { get; set; }
    [JsonPropertyName("comment")] public required string Comment { get; set; } = string.Empty;
}