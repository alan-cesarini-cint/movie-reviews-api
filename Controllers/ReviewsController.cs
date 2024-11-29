using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Models;
using Movies.Api.Repositories;

namespace Movies.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly IUserRepository _userRepository;

    public ReviewsController(IReviewRepository reviewRepository, IMovieRepository movieRepository, IUserRepository userRepository)
    {
        _reviewRepository = reviewRepository;
        _movieRepository = movieRepository;
        _userRepository = userRepository;
    }

    [HttpGet]
    public IActionResult GetReviews()
    {
        var reviews = _reviewRepository.GetReviewsAsync().Result;

        return Ok(reviews);
    }

    [HttpGet("by_movie/{id}")]
    public IActionResult GetReviewsByMovieId(string id)
    {
        var reviews = _reviewRepository.GetReviewsByMovieIdAsync(id).Result;
        if (!reviews.Any()) return NotFound();

        return Ok(reviews);
    }
    
    [HttpGet("by_user/{id}")]
    public IActionResult GetReviewsByUserId(string id)
    {
        var reviews = _reviewRepository.GetReviewsByUserIdAsync(id).Result;
        if (!reviews.Any()) return NotFound();

        return Ok(reviews);
    }

    [HttpGet("{id}")]
    public IActionResult GetReviewById(string id)
    {
        var review = _reviewRepository.GetReviewAsync(id).Result;
        if (review == null) return NotFound();

        return Ok(review);
    }

    [HttpPost]
    [Authorize(Policy = "UserPolicy")]
    public IActionResult AddReview([FromBody] Review newReview)
    {
        Console.WriteLine("MOVIE ID: " + newReview.MovieId);
        Console.WriteLine("USER ID: " + newReview.UserId);
        var movie = _movieRepository.GetMovieAsync(newReview.MovieId).Result;
        if (movie == null) return BadRequest();
        
        var user = _userRepository.GetUserAsync(newReview.UserId).Result;
        if (user == null) return BadRequest();
        
        /*newReview.Id = Guid.NewGuid().ToString();
        _reviewRepository.AddReviewAsync(newReview).Wait();*/

        return CreatedAtAction(nameof(GetReviewById), new { id = newReview.Id }, newReview);
    }
}