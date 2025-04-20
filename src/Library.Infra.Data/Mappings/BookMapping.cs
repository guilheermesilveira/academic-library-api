using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Infra.Data.Mappings;

public class BookMapping : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder
            .HasKey(b => b.Id);

        builder
            .Property(b => b.Title)
            .IsRequired()
            .HasColumnType("VARCHAR(100)");

        builder
            .Property(b => b.Author)
            .IsRequired()
            .HasColumnType("VARCHAR(50)");

        builder
            .Property(b => b.Edition)
            .IsRequired()
            .HasColumnType("VARCHAR(30)");

        builder
            .Property(b => b.Publisher)
            .IsRequired()
            .HasColumnType("VARCHAR(50)");

        builder
            .Property(b => b.Category)
            .IsRequired()
            .HasColumnType("VARCHAR(50)");

        builder
            .Property(b => b.Code)
            .IsRequired();

        builder
            .Property(b => b.YearOfPublication)
            .IsRequired();

        builder
            .Property(b => b.QuantityOfCopiesAvailableInStock)
            .IsRequired();

        builder
            .Property(b => b.QuantityOfCopiesAvailableForLoan)
            .IsRequired();

        builder
            .Property(b => b.BookStatus)
            .IsRequired()
            .HasColumnType("VARCHAR(20)");

        builder
            .Property(b => b.BookCover)
            .IsRequired(false)
            .HasColumnType("VARCHAR(255)");

        builder
            .Property(b => b.Active)
            .IsRequired();

        builder
            .Property(b => b.CreatedAt)
            .ValueGeneratedOnAdd()
            .HasColumnType("DATETIME");

        builder
            .Property(b => b.UpdatedAt)
            .ValueGeneratedOnAddOrUpdate()
            .HasColumnType("DATETIME");

        builder
            .HasMany(b => b.Loans)
            .WithOne(l => l.Book);
    }
}