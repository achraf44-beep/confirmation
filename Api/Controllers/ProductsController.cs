using Application.Products;
using Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Authorize(Policy = AuthPolicyNames.OrdersRead)]
[Route("api/[controller]")]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductAppService productService;

    public ProductsController(IProductAppService productService)
    {
        this.productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll(CancellationToken cancellationToken)
    {
        var products = await productService.GetAllAsync(cancellationToken);
        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var product = await productService.GetByIdAsync(id, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicyNames.OrdersWrite)]
    public async Task<ActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var id = await productService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthPolicyNames.OrdersWrite)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var updated = await productService.UpdateAsync(id, request, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = AuthPolicyNames.OrdersWrite)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await productService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{id:guid}/stock")]
    [Authorize(Policy = AuthPolicyNames.OrdersWrite)]
    public async Task<IActionResult> UpdateStock(Guid id, [FromBody] UpdateStockRequest request, CancellationToken cancellationToken)
    {
        var updated = await productService.UpdateStockAsync(id, request.Quantity, cancellationToken);
        return updated ? NoContent() : NotFound();
    }
}

public sealed record UpdateStockRequest(int Quantity);
