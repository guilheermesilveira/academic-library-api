using Library.Application.Contracts.Services;
using Library.Application.DTOs.Auth;
using Library.Application.Notifications;
using Library.Api.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Library.Api.Controllers;

[AllowAnonymous]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(INotificator notificator, IAuthService authService) : base(notificator)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [SwaggerOperation(Summary = "Authenticate", Tags = new[] { "Authentication" })]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _authService.Login(dto);
        return token != null ? OkResponse(token) : Unauthorized(new[] { "Incorrect email and/or password" });
    }
}