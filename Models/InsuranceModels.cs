namespace UssdInsuranceService.Models;

public class Customer
{
    public int Id { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string IdNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Policy
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public PolicyType Type { get; set; }
    public PolicyStatus Status { get; set; }
    public decimal Premium { get; set; }
    public decimal CoverAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? NextPaymentDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum PolicyType
{
    MicroInsurance,
    Health,
    Motor,
    Funeral,
    Life,
    Property
}

public enum PolicyStatus
{
    Active,
    Pending,
    Expired,
    Cancelled,
    Suspended
}

public class Claim
{
    public int Id { get; set; }
    public int PolicyId { get; set; }
    public string ClaimNumber { get; set; } = string.Empty;
    public ClaimType Type { get; set; }
    public ClaimStatus Status { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime IncidentDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
}

public enum ClaimType
{
    Medical,
    Accident,
    Death,
    Theft,
    Damage,
    Other
}

public enum ClaimStatus
{
    Submitted,
    UnderReview,
    Approved,
    Rejected,
    Paid
}

public class Payment
{
    public int Id { get; set; }
    public int PolicyId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public string TransactionReference { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Cancelled
}
