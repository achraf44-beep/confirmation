using Application.Identity.Abstractions;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Security;

public sealed class PasswordHasherService : IPasswordHasherService
{
    private readonly PasswordHasher<AppUser> passwordHasher = new();

    public string HashPassword(AppUser user, string password)
    {
        return passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(AppUser user, string hashedPassword, string providedPassword)
    {
        var result = passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}