using EcommerceAPI.DTOs;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService) => _categoryService = categoryService;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CategoryDTO>>> GetAll() =>
        Ok(await _categoryService.GetAllAsync());
}
