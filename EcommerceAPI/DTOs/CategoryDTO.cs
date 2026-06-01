namespace EcommerceAPI.DTOs;

public class CategoryDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateCategoryDTO
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateCategoryDTO
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
