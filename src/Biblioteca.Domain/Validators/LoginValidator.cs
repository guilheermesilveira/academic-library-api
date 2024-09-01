using Biblioteca.Domain.Entities;
using FluentValidation;

namespace Biblioteca.Domain.Validators;

public class LoginValidator : AbstractValidator<Administrador>
{
    public LoginValidator()
    {
        RuleFor(a => a.Email)
            .NotEmpty()
            .WithMessage("O email não pode ser vazio.");

        RuleFor(a => a.Senha)
            .NotEmpty()
            .WithMessage("A senha não pode ser vazia.");
    }
}