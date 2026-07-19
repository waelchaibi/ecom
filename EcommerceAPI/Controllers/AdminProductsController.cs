using EcommerceAPI.DTOs;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers;

[ApiController]
[Route("api/admin/products")]
[Authorize(Roles = "Admin")]
public class AdminProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IAuditService _audit;
    private readonly IProductImageStorage _images;

    public AdminProductsController(
        IProductService productService,
        IAuditService audit,
        IProductImageStorage images)
    {
        _productService = productService;
        _audit = audit;
        _images = images;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDTO>> Create([FromBody] CreateProductDTO dto)
    {
        var created = await _productService.CreateProductAsync(dto);
        await _audit.LogAsync(User.Identity?.Name ?? "admin", "Create", "Product", created.Id, created.Name);
        return Created($"/api/products/{created.Id}", created);
    }

    [HttpPost("image")]
    [RequestSizeLimit(6 * 1024 * 1024)]
    [ProducesResponseType(typeof(ProductImageUploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductImageUploadResponse>> UploadImage(IFormFile file, CancellationToken cancellationToken)
    {
        var path = await _images.SaveAsync(file, cancellationToken);
        return Ok(new ProductImageUploadResponse { ImageUrl = path });
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductDTO>> Update(int id, [FromBody] UpdateProductDTO dto)
    {
        var updated = await _productService.UpdateProductAsync(id, dto);
        await _audit.LogAsync(User.Identity?.Name ?? "admin", "Update", "Product", id);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteProductAsync(id);
        await _audit.LogAsync(User.Identity?.Name ?? "admin", "Delete", "Product", id);
        return NoContent();
    }

    [HttpPatch("{id:int}/stock")]
    [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductDTO>> UpdateStock(int id, [FromBody] UpdateStockDTO dto)
    {
        var updated = await _productService.UpdateStockAsync(id, dto.StockQuantity);
        return Ok(updated);
    }
}

public sealed class ProductImageUploadResponse
{
    public string ImageUrl { get; set; } = string.Empty;
}
