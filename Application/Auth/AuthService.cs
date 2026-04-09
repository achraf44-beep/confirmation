using Application.Auth.Abstractions;
using Application.Identity.Abstractions;
using Domain.Identity;

namespace Application.Auth;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository userRepository;
    private readonly IPasswordHasherService passwordHasherService;

    public AuthService(IUserRepository userRepository, IPasswordHasherService passwordHasherService)
    {
        this.userRepository = userRepository;
        this.passwordHasherService = passwordHasherService;
    }

    public async Task<AuthenticatedUserDto?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }

        var existing = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (existing is not null)
        {
            return null;
        }

        var role = ParseRole(request.Role);
        var user = AppUser.Create(request.Email, role);

        var passwordHash = passwordHasherService.HashPassword(user, request.Password);
        user.SetPasswordHash(passwordHash);

        user.AddClaim("permissions", "orders.read");

        if (role is UserRole.Owner || role is UserRole.Agent)
        {
            user.AddClaim("permissions", "orders.write");
        }

        if (role is UserRole.DeliveryCoordinator)
        {
            user.AddClaim("permissions", "shipments.read");
        }

        await userRepository.AddAsync(user, cancellationToken);

        return MapUser(user);
    }

    public async Task<AuthenticatedUserDto?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }

        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || !user.IsActive)
        {
            return null;
        }

        var isValid = passwordHasherService.VerifyPassword(user, user.PasswordHash, request.Password);

        if (!isValid)
        {
            return null;
        }

        return MapUser(user);
    }

    private static UserRole ParseRole(string role)
    {
        return Enum.TryParse<UserRole>(role, true, out var parsed)
            ? parsed
            : UserRole.Agent;
    }

    private static AuthenticatedUserDto MapUser(AppUser user)
    {
        return new AuthenticatedUserDto(
            user.Id,
            user.Email,
            user.Role.ToString(),
            user.Claims.Select(claim => new AuthClaimDto(claim.Type, claim.Value)).ToList());
    }
}