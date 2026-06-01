using EcommerceAPI.DTOs;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductDTO>>> GetAll([FromQuery] int? categoryId)
    {
        var list = await _productService.GetAllProductsAsync(categoryId);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDTO>> GetById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product is null)
            return NotFound();
        return Ok(product);
    }
}
