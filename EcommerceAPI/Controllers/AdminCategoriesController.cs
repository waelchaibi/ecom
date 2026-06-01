using EcommerceAPI.DTOs;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers;

[ApiController]
[Route("api/admin/categories")]
[Authorize(Roles = "Admin")]
public class AdminCategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly IAuditService _audit;

    public AdminCategoriesController(ICategoryService categoryService, IAuditService audit)
    {
        _categoryService = categoryService;
        _audit = audit;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDTO>>> List() =>
        Ok(await _categoryService.GetAllAsync());

    [HttpPost]
    public async Task<ActionResult<CategoryDTO>> Create([FromBody] CreateCategoryDTO dto)
    {
        var created = await _categoryService.CreateAsync(dto);
        await _audit.LogAsync(User.Identity?.Name ?? "admin", "Create", "Category", created.Id, created.Name);
        return CreatedAtAction(nameof(List), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryDTO>> Update(int id, [FromBody] UpdateCategoryDTO dto)
    {
        var updated = await _categoryService.UpdateAsync(id, dto);
        await _audit.LogAsync(User.Identity?.Name ?? "admin", "Update", "Category", id);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _categoryService.DeleteAsync(id);
        await _audit.LogAsync(User.Identity?.Name ?? "admin", "Delete", "Category", id);
        return NoContent();
    }
}
