using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Infra.Data.Mappings;

public class StudentMapping : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder
            .HasKey(s => s.Id);

        builder
            .Property(s => s.Name)
            .IsRequired()
            .HasColumnType("VARCHAR(50)");

        builder
            .Property(s => s.Registration)
            .IsRequired()
            .HasColumnType("CHAR(6)");

        builder
            .Property(s => s.Course)
            .IsRequired()
            .HasColumnType("VARCHAR(100)");

        builder
            .Property(s => s.Email)
            .IsRequired()
            .HasColumnType("VARCHAR(100)");

        builder
            .Property(s => s.Password)
            .IsRequired()
            .HasColumnType("VARCHAR(255)");

        builder
            .Property(s => s.NumberOfLoansAllowed)
            .IsRequired();

        builder
            .Property(s => s.NumberOfLoansTaken)
            .IsRequired();

        builder
            .Property(s => s.Blocked)
            .IsRequired();

        builder
            .Property(s => s.Active)
            .IsRequired();

        builder
            .Property(s => s.CreatedAt)
            .ValueGeneratedOnAdd()
            .HasColumnType("DATETIME");

        builder
            .Property(s => s.UpdatedAt)
            .ValueGeneratedOnAddOrUpdate()
            .HasColumnType("DATETIME");

        builder
            .HasMany(s => s.Loans)
            .WithOne(l => l.Student);
    }
}