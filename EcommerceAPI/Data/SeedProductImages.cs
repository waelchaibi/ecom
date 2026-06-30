namespace EcommerceAPI.Data;

/// <summary>Stable product image URLs (verified HTTP 200).</summary>
public static class SeedProductImages
{
    private static string Unsplash(string id) =>
        $"https://images.unsplash.com/photo-{id}?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&q=80";

    private static string Pexels(int id) =>
        $"https://images.pexels.com/photos/{id}/pexels-photo-{id}.jpeg?auto=compress&cs=tinysrgb&w=400&h=300&fit=crop";

    public static string Laptop => Unsplash("1496181133206-80ce9b88a853");
    public static string Mouse => Pexels(1067833);
    public static string Keyboard => Unsplash("1587829741301-dc798b83add3");
    public static string Monitor => Pexels(1714208);
    public static string UsbCable => Pexels(4386431);
}
