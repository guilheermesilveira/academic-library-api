using System.Reflection;
using Library.Domain.Contracts;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Infra.Data.Context;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Administrator> Administrators { get; set; } = null!;
    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<Loan> Loans { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

    public async Task<bool> Commit()
        => await SaveChangesAsync() > 0;
}