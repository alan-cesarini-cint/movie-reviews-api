using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Movies.Api.Models;
using Movies.Api.Repositories;

namespace Movies.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieRepository _movieRepository;

    public MoviesController(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
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
        var movie = _movieRepository.GetMovieAsync(id).Result;
        if (movie == null) return NotFound();

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
        if (!ModelState.IsValid) return BadRequest();

        _movieRepository.UpdateMovieAsync(id, updatedMovie).Wait();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    public IActionResult DeleteMovie(string id)
    {
        var movie = _movieRepository.GetMovieAsync(id).Result;
        if (movie == null) return NotFound();
        
        _movieRepository.DeleteMovieAsync(id).Wait();

        return NoContent();
    }
}