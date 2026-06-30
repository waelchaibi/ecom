namespace EcommerceAPI.DTOs;

/// <summary>Makeshift card payment — no external PSP; validates format and simulates approve/decline.</summary>
public class SimulatePaymentDTO
{
    public string CardholderName { get; set; } = string.Empty;
    public string CardNumber { get; set; } = string.Empty;
    public string ExpiryMonth { get; set; } = string.Empty;
    public string ExpiryYear { get; set; } = string.Empty;
    public string Cvv { get; set; } = string.Empty;
}

public class SimulatePaymentResultDTO
{
    public string TransactionId { get; set; } = string.Empty;
    public string Last4 { get; set; } = string.Empty;
    public string Status { get; set; } = "approved";
}
