namespace Application.Auth.Abstractions;

public interface IAuthService
{
    Task<AuthenticatedUserDto?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<AuthenticatedUserDto?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}