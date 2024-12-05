using Microsoft.AspNetCore.Mvc;
using Movies.Api.Models;
using Movies.Api.Repositories;
using Movies.Api.Utils;

namespace Movies.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository, IReviewRepository reviewRepository,
        ILogger<UsersController> logger)
    {
        _userRepository = userRepository;
        _reviewRepository = reviewRepository;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterUser([FromBody] User user)
    {
        await _userRepository.AddUserAsync(user);
        return Ok(user);
    }

    [HttpPost("login")]
    public IActionResult LoginUser([FromBody] LoginRequest loginRequest)
    {
        var user = _userRepository.GetUserByEmailAsync(loginRequest.Email).Result;
        if (user == null) return Unauthorized();

        if (!PasswordHasher.VerifyPassword(loginRequest.Password, user.Password)) return Unauthorized();

        var token = TokenHelper.GenerateToken(loginRequest.Email, user.Role);
        return Ok(new { Token = token });
    }

    [HttpGet("{id}")]
    public IActionResult GetUserById(string id)
    {
        var user = _userRepository.GetUserAsync(id).Result;
        if (user == null)
        {
            _logger.LogError($"User with id: {id} not found");
            return NotFound();
        }

        return Ok(user);
    }
}