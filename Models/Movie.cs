using System.Text.Json.Serialization;

namespace Movies.Api.Models;

public class Movie : IMovie
{
    [JsonPropertyName("id")] public string Id { get; set; } = Guid.NewGuid().ToString();
    [JsonPropertyName("title")] public required string Title { get; set; }
    [JsonPropertyName("year")] public required int Year { get; set; }
    [JsonPropertyName("genre")] public required string Genre { get; set; }
    [JsonPropertyName("synopsis")] public required string Synopsis { get; set; }
}