using Application.Delivery.Abstractions;
using Domain.Delivery;

namespace Application.Delivery;

public interface IDeliveryAppService
{
    Task<IReadOnlyList<DeliveryCompanyDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DeliveryCompanyDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(CreateDeliveryCompanyRequest request, CancellationToken cancellationToken = default);
}

public sealed class DeliveryAppService : IDeliveryAppService
{
    private readonly IDeliveryCompanyRepository repository;

    public DeliveryAppService(IDeliveryCompanyRepository repository)
    {
        this.repository = repository;
    }

    public async Task<IReadOnlyList<DeliveryCompanyDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var companies = await repository.GetAllAsync(cancellationToken);
        return companies.Where(c => c.IsActive).Select(MapToDto).ToList();
    }

    public async Task<DeliveryCompanyDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var company = await repository.GetByIdAsync(id, cancellationToken);
        return company is null ? null : MapToDto(company);
    }

    public async Task<Guid> CreateAsync(CreateDeliveryCompanyRequest request, CancellationToken cancellationToken = default)
    {
        var company = DeliveryCompany.Create(request.Name, request.Code, request.ApiEndpoint, request.ApiKey);
        await repository.AddAsync(company, cancellationToken);
        return company.Id;
    }

    private static DeliveryCompanyDto MapToDto(DeliveryCompany company) => new(
        company.Id,
        company.Name,
        company.Code,
        company.IsActive,
        company.CreatedAtUtc);
}
