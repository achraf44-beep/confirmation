using Application.Auth;
using Application.Auth.Abstractions;
using Api.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService authService;
    private readonly JwtTokenFactory tokenFactory;

    public AuthController(IAuthService authService, JwtTokenFactory tokenFactory)
    {
        this.authService = authService;
        this.tokenFactory = tokenFactory;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var user = await authService.RegisterAsync(request, cancellationToken);

        if (user is null)
        {
            return Conflict("Registration failed. Email may already exist or input is invalid.");
        }

        var token = tokenFactory.CreateToken(user);
        return Ok(new AuthResponse(token.Token, token.ExpiresAtUtc, user));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await authService.LoginAsync(request, cancellationToken);

        if (user is null)
        {
            return Unauthorized("Invalid email or password.");
        }

        var token = tokenFactory.CreateToken(user);
        return Ok(new AuthResponse(token.Token, token.ExpiresAtUtc, user));
    }

    [Authorize]
    [HttpGet("me")]
    [HttpGet("/api/me")]
    public ActionResult<object> Me()
    {
        var claims = User.Claims
            .Select(claim => new { claim.Type, claim.Value })
            .ToList();

        return Ok(new
        {
            userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
            email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value,
            role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value,
            claims
        });
    }

    [Authorize]
    [HttpPost("/identity/token/generate")]
    public ActionResult<TokenGenerationResponse> GenerateToken([FromBody] TokenGenerationRequest? request = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var role = User.FindFirstValue(ClaimTypes.Role);

        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(role))
        {
            return Unauthorized("Current token does not include required identity claims.");
        }

        var customClaims = User.Claims
            .Where(claim => claim.Type != ClaimTypes.NameIdentifier && claim.Type != ClaimTypes.Email && claim.Type != ClaimTypes.Role)
            .Select(claim => new AuthClaimDto(claim.Type, claim.Value))
            .ToList();

        var user = new AuthenticatedUserDto(Guid.Parse(userId), email, role, customClaims);
        var accessToken = tokenFactory.CreateToken(user);

        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);

        return Ok(new TokenGenerationResponse(
            accessToken.Token,
            accessToken.ExpiresAtUtc,
            refreshToken,
            refreshTokenExpiresAtUtc,
            request?.RefreshToken));
    }
}

public sealed record AuthResponse(string AccessToken, DateTime ExpiresAtUtc, AuthenticatedUserDto User);

public sealed record TokenGenerationRequest(string? RefreshToken);

public sealed record TokenGenerationResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc,
    string? PreviousRefreshToken);