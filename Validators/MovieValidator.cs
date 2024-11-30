using FluentValidation;
using Movies.Api.Models;

namespace Movies.Api.Validators;

public class MovieValidator : AbstractValidator<Movie>
{
    public MovieValidator()
    {
        RuleFor(movie => movie.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(movie => movie.Genre)
            .NotEmpty().WithMessage("Genre is required.")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

        RuleFor(movie => movie.Synopsis)
            .NotEmpty().WithMessage("Synopsis is required.")
            .MaximumLength(1000).WithMessage("Genre cannot exceed 1000 characters.");

        RuleFor(movie => movie.Year)
            .NotEmpty().WithMessage("Year is required.")
            .LessThanOrEqualTo(DateTime.Now.Year).WithMessage("Release date cannot be in the future.");
    }
}