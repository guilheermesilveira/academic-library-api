using Library.Application.DTOs.Auth;

namespace Library.Application.Contracts.Services;

public interface IAuthService
{
    Task<TokenDto?> Login(LoginDto dto);
}