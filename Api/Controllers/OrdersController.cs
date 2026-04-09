using Application.Orders;
using Application.Orders.Abstractions;
using Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Authorize(Policy = AuthPolicyNames.OrdersRead)]
[Route("api/[controller]")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderAppService orderAppService;

    public OrdersController(IOrderAppService orderAppService)
    {
        this.orderAppService = orderAppService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderListItemDto>>> GetAll(CancellationToken cancellationToken)
    {
        var orders = await orderAppService.GetAllAsync(cancellationToken);
        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDetailsDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var order = await orderAppService.GetByIdAsync(id, cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicyNames.OrdersWrite)]
    public async Task<ActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var id = await orderAppService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Policy = AuthPolicyNames.OrdersWrite)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequest request, CancellationToken cancellationToken)
    {
        var updated = await orderAppService.UpdateStatusAsync(id, request, cancellationToken);
        return updated ? NoContent() : NotFound();
    }
}