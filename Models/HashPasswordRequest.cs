using System.Text.Json.Serialization;

namespace Movies.Api.Models;

public class HashPasswordRequest
{
    [JsonPropertyName("password")] public required string Password { get; set; }
}