namespace Movies.Api.Models;

public interface IMovie
{
    string Id { get; }
    string Title { get; set; }
    int Year { get; set; }
    string Genre { get; set; }
    string Synopsis { get; set; }
}