using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Library.Domain.Contracts.Repositories;
using Library.Domain.Entities;
using Library.Domain.Validators;
using Library.Application.Configurations;
using Library.Application.Contracts.Services;
using Library.Application.DTOs.Auth;
using Library.Application.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.Jwt.Core.Interfaces;

namespace Library.Application.Services;

public class AuthService : BaseService, IAuthService
{
    private readonly IAdministratorRepository _administratorRepository;
    private readonly IPasswordHasher<Administrator> _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        INotificator notificator,
        IMapper mapper,
        IAdministratorRepository administratorRepository,
        IPasswordHasher<Administrator> passwordHasher,
        IJwtService jwtService,
        IOptions<JwtSettings> jwtSettings
    ) : base(notificator, mapper)
    {
        _administratorRepository = administratorRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<TokenDto?> Login(LoginDto dto)
    {
        if (!await LoginValidations(dto))
            return null;

        var administrator = await _administratorRepository.FirstOrDefault(a => a.Email == dto.Email);
        if (administrator == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(administrator, administrator.Password, dto.Password);
        if (result != PasswordVerificationResult.Failed)
        {
            return new TokenDto
            {
                Token = await GenerateToken(administrator)
            };
        }

        Notificator.Handle("Unable to log in");
        return null;
    }

    private async Task<string> GenerateToken(Administrator administrator)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, administrator.Id.ToString()));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, administrator.Name));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, administrator.Email));

        var key = await _jwtService.GetCurrentSigningCredentials();

        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,
            Expires = DateTime.UtcNow.AddHours(_jwtSettings.HoursUntilExpiry),
            SigningCredentials = key
        });

        return tokenHandler.WriteToken(token);
    }

    private async Task<bool> LoginValidations(LoginDto dto)
    {
        var administrator = Mapper.Map<Administrator>(dto);
        var validator = new LoginValidator();

        var result = await validator.ValidateAsync(administrator);
        if (!result.IsValid)
        {
            Notificator.Handle(result.Errors);
            return false;
        }

        return true;
    }
}