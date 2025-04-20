using FluentValidation;
using Library.Domain.Entities;

namespace Library.Domain.Validators;

public class LoginValidator : AbstractValidator<Administrator>
{
    public LoginValidator()
    {
        RuleFor(a => a.Email)
            .NotEmpty()
            .WithMessage("Email cannot be empty");

        RuleFor(a => a.Password)
            .NotEmpty()
            .WithMessage("Password cannot be empty");
    }
}