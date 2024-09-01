using Biblioteca.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biblioteca.Infra.Data.Mappings;

public class EmprestimoMapping : IEntityTypeConfiguration<Emprestimo>
{
    public void Configure(EntityTypeBuilder<Emprestimo> builder)
    {
        builder
            .HasKey(e => e.Id);

        builder
            .Property(e => e.DataEmprestimo)
            .IsRequired()
            .HasColumnType("DATE");

        builder
            .Property(e => e.DataDevolucaoPrevista)
            .IsRequired()
            .HasColumnType("DATE");

        builder
            .Property(e => e.DataDevolucaoRealizada)
            .IsRequired(false)
            .HasColumnType("DATE");

        builder
            .Property(e => e.StatusEmprestimo)
            .IsRequired()
            .HasColumnType("VARCHAR(20)");

        builder
            .Property(e => e.QuantidadeRenovacoesPermitida)
            .IsRequired();

        builder
            .Property(e => e.QuantidadeRenovacoesRealizadas)
            .IsRequired();

        builder
            .Property(e => e.AlunoId)
            .IsRequired();

        builder
            .Property(e => e.LivroId)
            .IsRequired();
        
        builder
            .Property(e => e.Ativo)
            .IsRequired();

        builder
            .Property(e => e.CriadoEm)
            .ValueGeneratedOnAdd()
            .HasColumnType("DATETIME");

        builder
            .Property(e => e.AtualizadoEm)
            .ValueGeneratedOnAddOrUpdate()
            .HasColumnType("DATETIME");
    }
}