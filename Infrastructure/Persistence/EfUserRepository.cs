using Application.Identity.Abstractions;
using Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class EfUserRepository : IUserRepository
{
    private readonly AppDbContext dbContext;

    public EfUserRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return await dbContext.Users
            .Include(user => user.Claims)
            .FirstOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    public async Task<AppUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .Include(user => user.Claims)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task AddAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}