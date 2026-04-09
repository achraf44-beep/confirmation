using Application.Delivery;
using Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Authorize(Policy = AuthPolicyNames.OrdersRead)]
[Route("api/[controller]")]
public sealed class DeliveryCompaniesController : ControllerBase
{
    private readonly IDeliveryAppService deliveryService;

    public DeliveryCompaniesController(IDeliveryAppService deliveryService)
    {
        this.deliveryService = deliveryService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DeliveryCompanyDto>>> GetAll(CancellationToken cancellationToken)
    {
        var companies = await deliveryService.GetAllAsync(cancellationToken);
        return Ok(companies);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DeliveryCompanyDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var company = await deliveryService.GetByIdAsync(id, cancellationToken);
        return company is null ? NotFound() : Ok(company);
    }
}
