using Domain.Identity;

namespace Application.Identity.Abstractions;

public interface IUserRepository
{
    Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<AppUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(AppUser user, CancellationToken cancellationToken = default);
}