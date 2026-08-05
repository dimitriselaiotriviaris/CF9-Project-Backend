using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CF9Project.DTO;
using CF9Project.Services;
using System.Security.Claims;

namespace CF9Project.Controllers.Api;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IApplicationService _applicationService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IApplicationService applicationService,
        ILogger<AuthController> logger)
    {
        _applicationService = applicationService;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO credentials)
    {
        var user = await _applicationService.UserService.VerifyAndGetUserAsync(credentials);

        if (user is null)
        {
            return Unauthorized(new
            {
                message = "Invalid username or password."
            });
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role.Name)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = credentials.KeepLoggedIn
            });

        _logger.LogInformation(
            "API login succeeded for {Username} with role {Role}",
            user.Username,
            user.Role.Name);

        return Ok(new
        {
            user.Id,
            user.Username,
            user.Email,
            role = user.Role.Name
        });
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        return Ok(new
        {
            id = User.FindFirstValue(ClaimTypes.NameIdentifier),
            username = User.Identity?.Name,
            role = User.FindFirstValue(ClaimTypes.Role),
            authenticated = User.Identity?.IsAuthenticated ?? false
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return NoContent();
    }
}
