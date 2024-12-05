using System.Text.Json.Serialization;

namespace Movies.Api.Models;

public class LoginRequest
{
    [JsonPropertyName("email")] public required string Email { get; set; }
    [JsonPropertyName("password")] public required string Password { get; set; }
}