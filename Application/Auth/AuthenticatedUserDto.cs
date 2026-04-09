namespace Application.Auth;

public sealed record AuthenticatedUserDto(
    Guid UserId,
    string Email,
    string Role,
    IReadOnlyList<AuthClaimDto> Claims);