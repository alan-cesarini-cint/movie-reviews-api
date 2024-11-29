using System.Text.Json.Serialization;

namespace Movies.Api.Models;

public class User : IUser
{
    [JsonPropertyName("id")] public string Id { get; set; } = Guid.NewGuid().ToString();
    [JsonPropertyName("name")] public required string Name { get; set; }
    [JsonPropertyName("email")] public required string Email { get; set; }
    [JsonPropertyName("password")] public required string Password { get; set; }
    [JsonPropertyName("role")] public string Role { get; set; } = "User";
}