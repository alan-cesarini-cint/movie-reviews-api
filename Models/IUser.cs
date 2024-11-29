namespace Movies.Api.Models;

public interface IUser
{
    string Id { get; }
    string Name { get; set; }
    string Email { get; set; }
    string Password { get; set; }
    string Role { get; set; }
}