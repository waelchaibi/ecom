using System.Globalization;
using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services;

/// <summary>Simulated payment gateway (dev/demo). Declines test card 4000000000000002.</summary>
public sealed class PaymentGatewayService : IPaymentGatewayService
{
    private const string DeclineTestCard = "4000000000000002";
    private readonly ILogger<PaymentGatewayService> _logger;

    public PaymentGatewayService(ILogger<PaymentGatewayService> logger)
    {
        _logger = logger;
    }

    public Task<SimulatePaymentResultDTO> ProcessCardPaymentAsync(
        SimulatePaymentDTO request,
        decimal amount,
        int orderId,
        CancellationToken cancellationToken = default)
    {
        if (amount <= 0)
            throw new ArgumentException("Payment amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(request.CardholderName))
            throw new ArgumentException("Cardholder name is required.");

        var digits = NormalizeCardNumber(request.CardNumber);
        if (digits.Length < 13 || digits.Length > 19)
            throw new ArgumentException("Card number must contain 13–19 digits.");

        if (!IsValidExpiry(request.ExpiryMonth, request.ExpiryYear))
            throw new ArgumentException("Card expiry is invalid or expired.");

        var cvv = request.Cvv?.Trim() ?? string.Empty;
        if (cvv.Length is < 3 or > 4 || !cvv.All(char.IsDigit))
            throw new ArgumentException("CVV must be 3 or 4 digits.");

        if (digits == DeclineTestCard)
            throw new InvalidOperationException("Payment declined by simulated gateway (test decline card).");

        var last4 = digits[^4..];
        var txnId = $"sim_{orderId}_{Guid.NewGuid():N}"[..24];

        _logger.LogInformation(
            "Simulated payment approved OrderId={OrderId} Amount={Amount} Last4={Last4} Txn={TxnId}",
            orderId, amount, last4, txnId);

        return Task.FromResult(new SimulatePaymentResultDTO
        {
            TransactionId = txnId,
            Last4 = last4,
            Status = "approved"
        });
    }

    private static string NormalizeCardNumber(string raw) =>
        new string((raw ?? string.Empty).Where(char.IsDigit).ToArray());

    private static bool IsValidExpiry(string monthRaw, string yearRaw)
    {
        if (!int.TryParse(monthRaw?.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var month))
            return false;
        if (month is < 1 or > 12)
            return false;

        var yearText = yearRaw?.Trim() ?? string.Empty;
        if (!int.TryParse(yearText, NumberStyles.None, CultureInfo.InvariantCulture, out var year))
            return false;

        if (yearText.Length == 2)
            year += 2000;

        if (year < 2000 || year > 2099)
            return false;

        var expiry = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
        return expiry.Date >= DateTime.UtcNow.Date;
    }
}
