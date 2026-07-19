using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace EcommerceAPI.Services;

public interface IProductImageStorage
{
    /// <summary>Saves an image under wwwroot/uploads/products and returns a public path like /uploads/products/{file}.</summary>
    Task<string> SaveAsync(IFormFile file, CancellationToken cancellationToken = default);
}

public sealed class ProductImageStorage : IProductImageStorage
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/webp",
        "image/gif"
    };

    private static readonly Dictionary<string, string> ExtensionByContentType = new(StringComparer.OrdinalIgnoreCase)
    {
        ["image/jpeg"] = ".jpg",
        ["image/jpg"] = ".jpg",
        ["image/png"] = ".png",
        ["image/webp"] = ".webp",
        ["image/gif"] = ".gif"
    };

    private const long MaxBytes = 5 * 1024 * 1024; // 5 MB

    private readonly IWebHostEnvironment _env;

    public ProductImageStorage(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> SaveAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Image file is required.");

        if (file.Length > MaxBytes)
            throw new ArgumentException("Image must be 5 MB or smaller.");

        var contentType = file.ContentType?.Trim() ?? string.Empty;
        if (!AllowedContentTypes.Contains(contentType))
            throw new ArgumentException("Only JPEG, PNG, WebP, or GIF images are allowed.");

        var ext = ExtensionByContentType[contentType];
        var fileName = $"{Guid.NewGuid():N}{ext}";
        var webRoot = _env.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRoot))
        {
            webRoot = Path.Combine(_env.ContentRootPath, "wwwroot");
            Directory.CreateDirectory(webRoot);
        }

        var folder = Path.Combine(webRoot, "uploads", "products");
        Directory.CreateDirectory(folder);

        var fullPath = Path.Combine(folder, fileName);
        await using (var stream = new FileStream(fullPath, FileMode.CreateNew))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        return $"/uploads/products/{fileName}";
    }
}
