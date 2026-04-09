using Domain.Identity;

namespace Application.Identity.Abstractions;

public interface IPasswordHasherService
{
    string HashPassword(AppUser user, string password);

    bool VerifyPassword(AppUser user, string hashedPassword, string providedPassword);
}