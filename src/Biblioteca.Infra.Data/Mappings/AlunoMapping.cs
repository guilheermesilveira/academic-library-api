using Biblioteca.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biblioteca.Infra.Data.Mappings;

public class AlunoMapping : IEntityTypeConfiguration<Aluno>
{
    public void Configure(EntityTypeBuilder<Aluno> builder)
    {
        builder
            .HasKey(a => a.Id);

        builder
            .Property(a => a.Nome)
            .IsRequired()
            .HasColumnType("VARCHAR(50)");

        builder
            .Property(a => a.Matricula)
            .IsRequired()
            .HasColumnType("CHAR(6)");

        builder
            .Property(a => a.Curso)
            .IsRequired()
            .HasColumnType("VARCHAR(100)");

        builder
            .Property(a => a.Email)
            .IsRequired()
            .HasColumnType("VARCHAR(100)");

        builder
            .Property(a => a.Senha)
            .IsRequired()
            .HasColumnType("VARCHAR(255)");

        builder
            .Property(a => a.QuantidadeEmprestimosPermitida)
            .IsRequired();

        builder
            .Property(a => a.QuantidadeEmprestimosRealizados)
            .IsRequired();

        builder
            .Property(a => a.Bloqueado)
            .IsRequired();

        builder
            .Property(a => a.Ativo)
            .IsRequired();

        builder
            .Property(a => a.CriadoEm)
            .ValueGeneratedOnAdd()
            .HasColumnType("DATETIME");

        builder
            .Property(a => a.AtualizadoEm)
            .ValueGeneratedOnAddOrUpdate()
            .HasColumnType("DATETIME");

        builder
            .HasMany(a => a.Emprestimos)
            .WithOne(e => e.Aluno);
    }
}