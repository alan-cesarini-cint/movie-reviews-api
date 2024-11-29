using Movies.Api.Models;

namespace Movies.Api.Repositories;

public interface IUserRepository
{
    Task<User> GetUserAsync(string id);
    Task<User> GetUserByEmailAsync(string email);
    Task AddUserAsync(User user);
}