using Biblioteca.Domain.Entities;
using FluentValidation;

namespace Biblioteca.Domain.Validators;

public class AlunoValidator : AbstractValidator<Aluno>
{
    public AlunoValidator()
    {
        RuleFor(a => a.Nome)
            .NotNull()
            .WithMessage("O nome não pode ser nulo.")
            .Length(3, 50)
            .WithMessage("O nome deve conter entre {MinLength} e {MaxLength} caracteres.");

        RuleFor(a => a.Matricula)
            .NotNull()
            .WithMessage("A matrícula não pode ser nula.")
            .Matches("^[0-9]{6}$")
            .WithMessage("A matrícula deve conter exatamente 6 dígitos numéricos.");

        RuleFor(a => a.Curso)
            .NotNull()
            .WithMessage("O curso não pode ser nulo.")
            .Length(3, 100)
            .WithMessage("O curso deve conter entre {MinLength} e {MaxLength} caracteres.");

        RuleFor(a => a.Email)
            .NotNull()
            .WithMessage("O email não pode ser nulo.")
            .EmailAddress()
            .WithMessage("O email fornecido não é válido.")
            .MaximumLength(100)
            .WithMessage("O email deve conter no máximo {MaxLength} caracteres.");

        RuleFor(a => a.Senha)
            .NotNull()
            .WithMessage("A senha não pode ser nula.")
            .Matches("^[0-9]{6}$")
            .WithMessage("A senha deve conter exatamente 6 dígitos numéricos.");
    }
}