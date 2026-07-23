using System.Text.Json;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services;

public interface IChatQueryTools
{
    Task<string> ExecuteAsync(string name, string argumentsJson, bool isAdmin, int? customerId, CancellationToken ct);
    IReadOnlyList<object> GetToolDefinitions(bool isAdmin);
}

public sealed class ChatQueryTools : IChatQueryTools
{
    private readonly AppDbContext _db;

    public ChatQueryTools(AppDbContext db) => _db = db;

    public IReadOnlyList<object> GetToolDefinitions(bool isAdmin)
    {
        var tools = new List<object>
        {
            Tool("search_products", "Search products by name or list catalog with stock and price.", new
            {
                type = "object",
                properties = new
                {
                    query = new { type = "string", description = "Optional name filter" },
                    take = new { type = "integer", description = "Max rows (default 10)" }
                }
            }),
            Tool("list_categories", "List product categories.", new { type = "object", properties = new { } }),
            Tool("get_product", "Get one product by id.", new
            {
                type = "object",
                properties = new { productId = new { type = "integer" } },
                required = new[] { "productId" }
            })
        };

        if (isAdmin)
        {
            tools.Add(Tool("search_customers", "Search customers (name/email/CIN).", new
            {
                type = "object",
                properties = new
                {
                    query = new { type = "string" },
                    take = new { type = "integer" }
                }
            }));
            tools.Add(Tool("list_orders", "List recent orders with optional status filter.", new
            {
                type = "object",
                properties = new
                {
                    status = new { type = "string" },
                    take = new { type = "integer" }
                }
            }));
            tools.Add(Tool("get_order", "Get order detail by id.", new
            {
                type = "object",
                properties = new { orderId = new { type = "integer" } },
                required = new[] { "orderId" }
            }));
            tools.Add(Tool("get_analytics_summary", "Sales KPIs: revenue, order count, AOV (excludes Pending/Cancelled).", new
            {
                type = "object",
                properties = new { }
            }));
            tools.Add(Tool("list_gift_rules", "List gift rules and linked gifts.", new { type = "object", properties = new { } }));
            tools.Add(Tool("list_gifts", "List gift catalog and stock.", new { type = "object", properties = new { } }));
            tools.Add(Tool("list_audit_logs", "Recent audit log entries.", new
            {
                type = "object",
                properties = new { take = new { type = "integer" } }
            }));
        }
        else
        {
            tools.Add(Tool("get_my_profile", "Current customer profile including CIN.", new { type = "object", properties = new { } }));
            tools.Add(Tool("get_my_orders", "Current customer's orders.", new
            {
                type = "object",
                properties = new { take = new { type = "integer" } }
            }));
            tools.Add(Tool("get_my_order", "One of the current customer's orders by id.", new
            {
                type = "object",
                properties = new { orderId = new { type = "integer" } },
                required = new[] { "orderId" }
            }));
        }

        return tools;
    }

    public async Task<string> ExecuteAsync(string name, string argumentsJson, bool isAdmin, int? customerId, CancellationToken ct)
    {
        using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(argumentsJson) ? "{}" : argumentsJson);
        var root = doc.RootElement;

        return name switch
        {
            "search_products" => await SearchProducts(root, ct),
            "list_categories" => await ListCategories(ct),
            "get_product" => await GetProduct(root, ct),
            "get_my_profile" => await GetMyProfile(isAdmin, customerId, ct),
            "get_my_orders" => await GetMyOrders(isAdmin, customerId, root, ct),
            "get_my_order" => await GetMyOrder(isAdmin, customerId, root, ct),
            "search_customers" => await SearchCustomers(isAdmin, root, ct),
            "list_orders" => await ListOrders(isAdmin, root, ct),
            "get_order" => await GetOrder(isAdmin, root, ct),
            "get_analytics_summary" => await AnalyticsSummary(isAdmin, ct),
            "list_gift_rules" => await ListGiftRules(isAdmin, ct),
            "list_gifts" => await ListGifts(isAdmin, ct),
            "list_audit_logs" => await ListAudit(isAdmin, root, ct),
            _ => Json(new { error = $"Unknown tool: {name}" })
        };
    }

    private async Task<string> SearchProducts(JsonElement root, CancellationToken ct)
    {
        var q = root.TryGetProperty("query", out var qe) ? qe.GetString() : null;
        var take = Clamp(root, "take", 10, 25);
        var query = _db.Products.AsNoTracking().Include(p => p.Category).Where(p => p.IsActive).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(p => p.Name.Contains(q));
        var rows = await query.OrderBy(p => p.Name).Take(take)
            .Select(p => new { p.Id, p.Name, p.Price, p.StockQuantity, Category = p.Category != null ? p.Category.Name : null })
            .ToListAsync(ct);
        return Json(rows);
    }

    private async Task<string> ListCategories(CancellationToken ct)
    {
        var rows = await _db.Categories.AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new { c.Id, c.Name, c.Description })
            .ToListAsync(ct);
        return Json(rows);
    }

    private async Task<string> GetProduct(JsonElement root, CancellationToken ct)
    {
        if (!root.TryGetProperty("productId", out var idEl) || !idEl.TryGetInt32(out var id))
            return Json(new { error = "productId required" });
        var p = await _db.Products.AsNoTracking().Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        return p is null
            ? Json(new { error = "Not found" })
            : Json(new { p.Id, p.Name, p.Description, p.Price, p.StockQuantity, Category = p.Category?.Name });
    }

    private async Task<string> GetMyProfile(bool isAdmin, int? customerId, CancellationToken ct)
    {
        if (isAdmin || customerId is null)
            return Json(new { error = "Customer only" });
        var c = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == customerId, ct);
        return c is null
            ? Json(new { error = "Not found" })
            : Json(new { c.Id, c.Name, c.Email, c.Phone, c.IdentityCard });
    }

    private async Task<string> GetMyOrders(bool isAdmin, int? customerId, JsonElement root, CancellationToken ct)
    {
        if (isAdmin || customerId is null)
            return Json(new { error = "Customer only" });
        var take = Clamp(root, "take", 10, 30);
        var rows = await _db.Orders.AsNoTracking()
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.Id)
            .Take(take)
            .Select(o => new { o.Id, o.Status, o.TotalAmount, o.CreatedAt, o.PromotionCode })
            .ToListAsync(ct);
        return Json(rows);
    }

    private async Task<string> GetMyOrder(bool isAdmin, int? customerId, JsonElement root, CancellationToken ct)
    {
        if (isAdmin || customerId is null)
            return Json(new { error = "Customer only" });
        if (!root.TryGetProperty("orderId", out var idEl) || !idEl.TryGetInt32(out var id))
            return Json(new { error = "orderId required" });
        var o = await _db.Orders.AsNoTracking()
            .Include(x => x.OrderItems)
            .Include(x => x.OrderGifts)
            .FirstOrDefaultAsync(x => x.Id == id && x.CustomerId == customerId, ct);
        if (o is null) return Json(new { error = "Not found" });
        return Json(new
        {
            o.Id,
            o.Status,
            o.SubtotalAmount,
            o.TaxAmount,
            o.ShippingAmount,
            o.TotalAmount,
            o.PromotionCode,
            Items = o.OrderItems.Select(i => new { i.ProductId, i.Quantity, i.Price }),
            Gifts = o.OrderGifts.Select(g => new { g.GiftId, g.Quantity })
        });
    }

    private async Task<string> SearchCustomers(bool isAdmin, JsonElement root, CancellationToken ct)
    {
        if (!isAdmin) return Json(new { error = "Admin only" });
        var q = root.TryGetProperty("query", out var qe) ? qe.GetString() ?? "" : "";
        var take = Clamp(root, "take", 10, 25);
        var query = _db.Customers.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(c => c.Name.Contains(q) || c.Email.Contains(q) || c.IdentityCard.Contains(q));
        var rows = await query.OrderBy(c => c.Id).Take(take)
            .Select(c => new { c.Id, c.Name, c.Email, c.Phone, c.IdentityCard })
            .ToListAsync(ct);
        return Json(rows);
    }

    private async Task<string> ListOrders(bool isAdmin, JsonElement root, CancellationToken ct)
    {
        if (!isAdmin) return Json(new { error = "Admin only" });
        var take = Clamp(root, "take", 15, 40);
        var status = root.TryGetProperty("status", out var se) ? se.GetString() : null;
        var query = _db.Orders.AsNoTracking().Include(o => o.Customer).AsQueryable();
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(o => o.Status == status);
        var rows = await query.OrderByDescending(o => o.Id).Take(take)
            .Select(o => new { o.Id, o.Status, o.TotalAmount, Customer = o.Customer!.Name, o.CreatedAt })
            .ToListAsync(ct);
        return Json(rows);
    }

    private async Task<string> GetOrder(bool isAdmin, JsonElement root, CancellationToken ct)
    {
        if (!isAdmin) return Json(new { error = "Admin only" });
        if (!root.TryGetProperty("orderId", out var idEl) || !idEl.TryGetInt32(out var id))
            return Json(new { error = "orderId required" });
        var o = await _db.Orders.AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (o is null) return Json(new { error = "Not found" });
        return Json(new
        {
            o.Id,
            o.Status,
            o.TotalAmount,
            Customer = new { o.Customer!.Id, o.Customer.Name, o.Customer.Email, o.Customer.IdentityCard },
            Items = o.OrderItems.Select(i => new { i.ProductId, i.Quantity, i.Price })
        });
    }

    private async Task<string> AnalyticsSummary(bool isAdmin, CancellationToken ct)
    {
        if (!isAdmin) return Json(new { error = "Admin only" });
        var revenueOrders = _db.Orders.AsNoTracking()
            .Where(o => o.Status != OrderStatuses.Cancelled && o.Status != OrderStatuses.Pending);
        var totalRevenue = await revenueOrders.SumAsync(o => (decimal?)o.TotalAmount, ct) ?? 0m;
        var orderCount = await revenueOrders.CountAsync(ct);
        var aov = orderCount == 0 ? 0m : Math.Round(totalRevenue / orderCount, 2);
        return Json(new { totalRevenue, orderCount, averageOrderValue = aov });
    }

    private async Task<string> ListGiftRules(bool isAdmin, CancellationToken ct)
    {
        if (!isAdmin) return Json(new { error = "Admin only" });
        var rows = await _db.GiftRules.AsNoTracking().Include(r => r.Gift)
            .OrderBy(r => r.Priority)
            .Select(r => new { r.Id, r.RuleType, r.ConditionValue, r.Priority, r.IsActive, Gift = r.Gift!.Name })
            .ToListAsync(ct);
        return Json(rows);
    }

    private async Task<string> ListGifts(bool isAdmin, CancellationToken ct)
    {
        if (!isAdmin) return Json(new { error = "Admin only" });
        var rows = await _db.Gifts.AsNoTracking()
            .Select(g => new { g.Id, g.Name, g.StockQuantity, g.Description })
            .ToListAsync(ct);
        return Json(rows);
    }

    private async Task<string> ListAudit(bool isAdmin, JsonElement root, CancellationToken ct)
    {
        if (!isAdmin) return Json(new { error = "Admin only" });
        var take = Clamp(root, "take", 15, 40);
        var rows = await _db.AuditLogs.AsNoTracking()
            .OrderByDescending(a => a.CreatedAt)
            .Take(take)
            .Select(a => new { a.Id, a.AdminUsername, a.Action, a.EntityType, a.EntityId, a.CreatedAt, a.Details })
            .ToListAsync(ct);
        return Json(rows);
    }

    private static object Tool(string name, string description, object parameters) => new
    {
        type = "function",
        function = new { name, description, parameters }
    };

    private static int Clamp(JsonElement root, string prop, int def, int max)
    {
        if (root.TryGetProperty(prop, out var el) && el.TryGetInt32(out var v) && v > 0)
            return Math.Min(v, max);
        return def;
    }

    private static string Json(object value) =>
        JsonSerializer.Serialize(value, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
}
