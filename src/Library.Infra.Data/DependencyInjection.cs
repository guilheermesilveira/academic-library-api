using Library.Domain.Contracts.Repositories;
using Library.Infra.Data.Context;
using Library.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Infra.Data;

public static class DependencyInjection
{
    public static void ConfigInfra(this IServiceCollection services, IConfiguration configuration)
    {
        ConfigDataBase(services, configuration);
        ConfigRepositoryDependency(services);
    }

    private static void ConfigDataBase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var serverVersion = ServerVersion.AutoDetect(connectionString);

            options.UseMySql(connectionString, serverVersion);
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
        });
    }

    private static void ConfigRepositoryDependency(this IServiceCollection services)
    {
        services
            .AddScoped<IAdministratorRepository, AdministratorRepository>()
            .AddScoped<IStudentRepository, StudentRepository>()
            .AddScoped<IBookRepository, BookRepository>()
            .AddScoped<ILoanRepository, LoanRepository>();
    }
}