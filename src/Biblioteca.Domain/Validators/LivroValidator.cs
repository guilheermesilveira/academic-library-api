﻿using Biblioteca.Domain.Entities;
using FluentValidation;

namespace Biblioteca.Domain.Validators;

public class LivroValidator : AbstractValidator<Livro>
{
    public LivroValidator()
    {
        RuleFor(l => l.Titulo)
            .NotNull()
            .WithMessage("O título não pode ser nulo.")
            .Length(3, 100)
            .WithMessage("O título deve conter entre {MinLength} e {MaxLength} caracteres.");

        RuleFor(l => l.Autor)
            .NotNull()
            .WithMessage("O autor não pode ser nulo.")
            .Length(3, 50)
            .WithMessage("O autor deve conter entre {MinLength} e {MaxLength} caracteres.");

        RuleFor(l => l.Edicao)
            .NotNull()
            .WithMessage("A edição não pode ser nula.")
            .Length(3, 30)
            .WithMessage("A edição deve conter entre {MinLength} e {MaxLength} caracteres.");

        RuleFor(l => l.Editora)
            .NotNull()
            .WithMessage("A editora não pode ser nula.")
            .Length(3, 50)
            .WithMessage("A editora deve conter entre {MinLength} e {MaxLength} caracteres.");

        RuleFor(l => l.Categoria)
            .NotNull()
            .WithMessage("A categoria não pode ser nula.")
            .Length(3, 50)
            .WithMessage("A categoria deve conter entre {MinLength} e {MaxLength} caracteres.");

        RuleFor(l => l.AnoPublicacao)
            .NotNull()
            .WithMessage("O ano de publicação não pode ser nulo.")
            .GreaterThan(0)
            .WithMessage("O ano de publicação deve ser maior que 0.")
            .LessThanOrEqualTo(DateTime.Now.Year)
            .WithMessage("O ano de publicação não pode ser no futuro.");

        RuleFor(l => l.QuantidadeExemplaresDisponiveisEmEstoque)
            .NotNull()
            .WithMessage("A quantidade de exemplares não pode ser nula.")
            .GreaterThan(0)
            .WithMessage("A quantidade de exemplares deve ser maior que 0.");
    }
}