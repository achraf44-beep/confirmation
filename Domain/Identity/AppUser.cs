using Domain.Common;

namespace Domain.Identity;

public sealed class AppUser : AggregateRoot
{
    private readonly List<UserClaim> claims = [];

    private AppUser(
        Guid id,
        string email,
        string passwordHash,
        UserRole role,
        bool isActive,
        DateTime createdAtUtc)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }

        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        Role = role;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc;
    }

    public string Email { get; private set; }

    public string PasswordHash { get; private set; }

    public UserRole Role { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public IReadOnlyCollection<UserClaim> Claims => claims.AsReadOnly();

    public static AppUser Create(string email, UserRole role, DateTime? createdAtUtc = null)
    {
        return new AppUser(
            Guid.NewGuid(),
            email,
            string.Empty,
            role,
            true,
            createdAtUtc ?? DateTime.UtcNow);
    }

    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));
        }

        PasswordHash = passwordHash;
    }

    public void AddClaim(string type, string value)
    {
        if (claims.Any(existing => existing.Type == type && existing.Value == value))
        {
            return;
        }

        claims.Add(new UserClaim(type, value));
    }
}