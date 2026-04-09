namespace Application.Delivery;

public sealed record DeliveryCompanyDto(
    Guid Id,
    string Name,
    string Code,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record CreateDeliveryCompanyRequest(
    string Name,
    string Code,
    string? ApiEndpoint = null,
    string? ApiKey = null);
