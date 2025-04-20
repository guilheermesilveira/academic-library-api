using FluentValidation;
using Library.Domain.Entities;

namespace Library.Domain.Validators;

public class StudentValidator : AbstractValidator<Student>
{
    public StudentValidator()
    {
        RuleFor(s => s.Name)
            .NotNull()
            .WithMessage("The name cannot be null")
            .Length(3, 50)
            .WithMessage("The name must contain between {MinLength} and {MaxLength} characters");

        RuleFor(s => s.Registration)
            .NotNull()
            .WithMessage("Registration cannot be void")
            .Matches("^[0-9]{6}$")
            .WithMessage("The registration number must contain exactly 6 numeric digits");

        RuleFor(s => s.Course)
            .NotNull()
            .WithMessage("The course cannot be void")
            .Length(3, 100)
            .WithMessage("The course must contain between {MinLength} and {MaxLength} characters");

        RuleFor(s => s.Email)
            .NotNull()
            .WithMessage("Email cannot be null")
            .EmailAddress()
            .WithMessage("The email provided is not valid")
            .MaximumLength(100)
            .WithMessage("Email must contain a maximum of {MaxLength} characters");

        RuleFor(s => s.Password)
            .NotNull()
            .WithMessage("Password cannot be null")
            .Matches("^[0-9]{6}$")
            .WithMessage("The password must contain exactly 6 numeric digits");
    }
}