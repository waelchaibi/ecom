namespace EcommerceAPI.Models;

public class Cart
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Customer? Customer { get; set; }
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}

public class CartItem
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    public Cart? Cart { get; set; }
    public Product? Product { get; set; }
}
