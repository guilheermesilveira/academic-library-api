using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Infra.Data.Mappings;

public class LoanMapping : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder
            .HasKey(l => l.Id);

        builder
            .Property(l => l.LoanDate)
            .IsRequired()
            .HasColumnType("DATE");

        builder
            .Property(l => l.ExpectedDeliveryDate)
            .IsRequired()
            .HasColumnType("DATE");

        builder
            .Property(l => l.ActualDeliveryDate)
            .IsRequired(false)
            .HasColumnType("DATE");

        builder
            .Property(l => l.LoanStatus)
            .IsRequired()
            .HasColumnType("VARCHAR(20)");

        builder
            .Property(l => l.NumberOfRenewalsAllowed)
            .IsRequired();

        builder
            .Property(l => l.NumberOfRenewalsCompleted)
            .IsRequired();

        builder
            .Property(l => l.StudentId)
            .IsRequired();

        builder
            .Property(l => l.BookId)
            .IsRequired();
        
        builder
            .Property(l => l.Active)
            .IsRequired();

        builder
            .Property(l => l.CreatedAt)
            .ValueGeneratedOnAdd()
            .HasColumnType("DATETIME");

        builder
            .Property(l => l.UpdatedAt)
            .ValueGeneratedOnAddOrUpdate()
            .HasColumnType("DATETIME");
    }
}