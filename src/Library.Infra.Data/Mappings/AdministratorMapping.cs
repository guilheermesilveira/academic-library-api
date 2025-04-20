using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Infra.Data.Mappings;

public class AdministratorMapping : IEntityTypeConfiguration<Administrator>
{
    public void Configure(EntityTypeBuilder<Administrator> builder)
    {
        builder
            .HasKey(a => a.Id);

        builder
            .Property(a => a.Name)
            .IsRequired()
            .HasColumnType("VARCHAR(50)");

        builder
            .Property(a => a.Email)
            .IsRequired()
            .HasColumnType("VARCHAR(100)");
        
        builder
            .Property(a => a.Password)
            .IsRequired()
            .HasColumnType("VARCHAR(255)");
        
        builder
            .Property(a => a.Active)
            .IsRequired()
            .HasDefaultValue(true);

        builder
            .Property(a => a.CreatedAt)
            .ValueGeneratedOnAdd()
            .HasColumnType("DATETIME");

        builder
            .Property(a => a.UpdatedAt)
            .ValueGeneratedOnAddOrUpdate()
            .HasColumnType("DATETIME");
    }
}