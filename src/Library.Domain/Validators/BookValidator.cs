using FluentValidation;
using Library.Domain.Entities;

namespace Library.Domain.Validators;

public class BookValidator : AbstractValidator<Book>
{
    public BookValidator()
    {
        RuleFor(b => b.Title)
            .NotNull()
            .WithMessage("The title cannot be null")
            .Length(3, 100)
            .WithMessage("The title must contain between {MinLength} and {MaxLength} characters");

        RuleFor(b => b.Author)
            .NotNull()
            .WithMessage("The author cannot be null")
            .Length(3, 50)
            .WithMessage("The author must contain between {MinLength} and {MaxLength} characters");

        RuleFor(b => b.Edition)
            .NotNull()
            .WithMessage("The edition cannot be null")
            .Length(3, 30)
            .WithMessage("The edit must contain between {MinLength} and {MaxLength} characters");

        RuleFor(b => b.Publisher)
            .NotNull()
            .WithMessage("The publisher cannot be null")
            .Length(3, 50)
            .WithMessage("The publisher must contain between {MinLength} and {MaxLength} characters");

        RuleFor(b => b.Category)
            .NotNull()
            .WithMessage("The category cannot be null")
            .Length(3, 50)
            .WithMessage("The category must contain between {MinLength} and {MaxLength} characters");

        RuleFor(b => b.YearOfPublication)
            .NotNull()
            .WithMessage("The year of publication cannot be null")
            .GreaterThan(0)
            .WithMessage("Publication year must be greater than 0")
            .LessThanOrEqualTo(DateTime.Now.Year)
            .WithMessage("The year of publication cannot be in the future");

        RuleFor(b => b.QuantityOfCopiesAvailableInStock)
            .NotNull()
            .WithMessage("The number of copies cannot be null")
            .GreaterThan(0)
            .WithMessage("The number of copies must be greater than 0");
    }
}