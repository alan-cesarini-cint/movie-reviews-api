using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Models;
using Movies.Api.Repositories;

namespace Movies.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IMovieRepository _movieRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;

    public ReviewsController(IReviewRepository reviewRepository, IMovieRepository movieRepository,
        IUserRepository userRepository, ILogger<ReviewsController> logger)
    {
        _reviewRepository = reviewRepository;
        _movieRepository = movieRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetReviews()
    {
        var reviews = _reviewRepository.GetReviewsAsync().Result;

        return Ok(reviews);
    }

    [HttpGet("{id}")]
    public IActionResult GetReviewById(string id)
    {
        var review = _reviewRepository.GetReviewAsync(id).Result;
        if (review == null)
        {
            _logger.LogError("Review with ID {Id} not found.", id);
            return NotFound();
        }

        return Ok(review);
    }

    [HttpPost]
    [Authorize(Policy = "UserPolicy")]
    public IActionResult AddReview([FromBody] Review newReview)
    {
        var movie = _movieRepository.GetMovieAsync(newReview.MovieId).Result;
        if (movie == null) return BadRequest();

        var user = _userRepository.GetUserAsync(newReview.UserId).Result;
        if (user == null) return BadRequest();

        newReview.Id = Guid.NewGuid().ToString();
        _reviewRepository.AddReviewAsync(newReview).Wait();

        return CreatedAtAction(nameof(GetReviewById), new { id = newReview.Id }, newReview);
    }
}