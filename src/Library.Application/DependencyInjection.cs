using System.Reflection;
using Library.Domain.Entities;
using Library.Infra.Data;
using Library.Application.Configurations;
using Library.Application.Contracts.Services;
using Library.Application.Notifications;
using Library.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScottBrady91.AspNetCore.Identity;

namespace Library.Application;

public static class DependencyInjection
{
    public static void ConfigApplication(this IServiceCollection services, IConfiguration configuration)
    {
        ConfigJwtAndStorage(services, configuration);
        ConfigServiceDependency(services);
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.ConfigInfra(configuration);
    }

    private static void ConfigJwtAndStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<StorageSettings>(configuration.GetSection("StorageSettings"));
    }

    private static void ConfigServiceDependency(this IServiceCollection services)
    {
        services
            .AddScoped<IPasswordHasher<Administrator>, Argon2PasswordHasher<Administrator>>()
            .AddScoped<IPasswordHasher<Student>, Argon2PasswordHasher<Student>>()
            .AddScoped<INotificator, Notificator>();

        services
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IStudentService, StudentService>()
            .AddScoped<IBookService, BookService>()
            .AddScoped<ILoanService, LoanService>();
    }
}