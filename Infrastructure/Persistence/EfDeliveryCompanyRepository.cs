using Application.Delivery.Abstractions;
using Domain.Delivery;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class EfDeliveryCompanyRepository : IDeliveryCompanyRepository
{
    private readonly AppDbContext dbContext;

    public EfDeliveryCompanyRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IReadOnlyList<DeliveryCompany>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.DeliveryCompanies.ToListAsync(cancellationToken);
    }

    public async Task<DeliveryCompany?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.DeliveryCompanies.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<DeliveryCompany?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.ToUpperInvariant();
        return await dbContext.DeliveryCompanies.FirstOrDefaultAsync(c => c.Code == normalizedCode, cancellationToken);
    }

    public async Task AddAsync(DeliveryCompany company, CancellationToken cancellationToken = default)
    {
        dbContext.DeliveryCompanies.Add(company);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(DeliveryCompany company, CancellationToken cancellationToken = default)
    {
        dbContext.DeliveryCompanies.Update(company);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
