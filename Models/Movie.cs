using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Movies.Api.Models;

public class Movie : IMovie
{
    [JsonPropertyName("id")] public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required(ErrorMessage = "The Title field is required.")]
    [StringLength(200, ErrorMessage = "The Title cannot exceed 200 characters.")]
    [JsonPropertyName("title")] public required string Title { get; set; }

    [Required(ErrorMessage = "The Year field is required.")]
    [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
    [JsonPropertyName("year")] public required int Year { get; set; }

    [Required(ErrorMessage = "The Genre field is required.")]
    [StringLength(100, ErrorMessage = "The Genre cannot exceed 100 characters.")]
    [JsonPropertyName("genre")] public required string Genre { get; set; }
    [Required(ErrorMessage = "The Synopsis field is required.")]
    [StringLength(1000, ErrorMessage = "The Synopsis cannot exceed 1000 characters.")]
    [JsonPropertyName("synopsis")] public required string Synopsis { get; set; }
}