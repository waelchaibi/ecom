namespace EcommerceAPI.Models;

/// <summary>String values persisted on <see cref="Order.Status"/>.</summary>
public static class OrderStatuses
{
    public const string Pending = "Pending";
    public const string Confirmed = "Confirmed";
    public const string Shipped = "Shipped";
    public const string Cancelled = "Cancelled";
}

public static class OrderStatusHelper
{
    /// <summary>Prior orders in this state count toward loyalty rules (current cart order is not persisted yet).</summary>
    public static bool CountsTowardLoyaltyHistory(string status) =>
        status == OrderStatuses.Confirmed || status == OrderStatuses.Shipped;

    /// <summary>Orders included in revenue / AOV / customer rollups.</summary>
    public static bool CountsTowardRevenue(string status) =>
        status != OrderStatuses.Cancelled && status != OrderStatuses.Pending;
}
