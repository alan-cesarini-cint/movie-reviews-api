using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Models;
using Movies.Api.Repositories;

namespace Movies.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IMovieRepository _movieRepository;

    public MoviesController(IMovieRepository movieRepository, ILogger<MoviesController> logger)
    {
        _movieRepository = movieRepository;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetMovies([FromQuery] string? query = null)
    {
        var movies = query != null
            ? _movieRepository.GetMoviesBySearchQueryAsync(query).Result
            : _movieRepository.GetMoviesAsync().Result;
        return Ok(movies);
    }

    [HttpGet("{id}")]
    public IActionResult GetMovieById(string id)
    {
        var notFoundResult = GetMovieOrNotFound(id, out var movie);
        if (notFoundResult != null) return notFoundResult;

        return Ok(movie);
    }

    [HttpPost]
    [Authorize(Policy = "AdminPolicy")]
    public IActionResult AddMovie([FromBody] Movie newMovie)
    {
        if (!ModelState.IsValid) return BadRequest();

        _movieRepository.AddMovieAsync(newMovie).Wait();

        return CreatedAtAction(nameof(GetMovieById), new { id = newMovie.Id }, newMovie);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    public IActionResult UpdateMovie(string id, [FromBody] Movie updatedMovie)
    {
        var notFoundResult = GetMovieOrNotFound(id, out var movie);
        if (notFoundResult != null) return notFoundResult;

        if (!ModelState.IsValid) return BadRequest();

        _movieRepository.UpdateMovieAsync(id, updatedMovie).Wait();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    public IActionResult DeleteMovie(string id)
    {
        var notFoundResult = GetMovieOrNotFound(id, out var movie);
        if (notFoundResult != null) return notFoundResult;

        _movieRepository.DeleteMovieAsync(id).Wait();

        return NoContent();
    }

    private IActionResult GetMovieOrNotFound(string id, out Movie? movie)
    {
        movie = _movieRepository.GetMovieAsync(id).Result;

        if (movie == null)
        {
            _logger.LogError("Movie with ID {Id} not found.", id);
            return NotFound();
        }

        return null;
    }
}