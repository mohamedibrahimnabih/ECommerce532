namespace ECommerce532.Models;

public enum OrderStatus 
{
    Pending,
    InProcessing,
    Shipped,
    OnTheWay,
    Completed,
    Canceled
}

public enum PaymentMethod
{
    Mezza,
    Visa,
    MasterCard,
    PayPal,
    Stripe,
    COD
}

public enum PaymentStatus
{
    Pending,
    Succussed,
    Canceled,
    Refused,
    Refunded
}

public class Order : Audit
{
    public int Id { get; set; }
    public decimal TotalPrice { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }

    public string? CarrierName { get; set; }
    public string? TrackingNumber { get; set; }

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Stripe;
    public PaymentStatus PaymentStatus { get; set; }
    public string? TransactionId { get; set; }
    public string? SessionId { get; set; }
    public DateTime? PaymentDate { get; set; }
}
