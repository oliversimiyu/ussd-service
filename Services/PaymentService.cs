using UssdInsuranceService.Models;

namespace UssdInsuranceService.Services;

public class PaymentService : IPaymentService
{
    private static readonly List<Payment> _payments = new();
    private static int _paymentIdCounter = 1;
    private readonly IPolicyService _policyService;

    public PaymentService(IPolicyService policyService)
    {
        _policyService = policyService;
    }

    public async Task<Payment> InitiatePayment(int policyId)
    {
        var policies = await _policyService.GetCustomerPolicies(0); // Simplified
        var policy = policies.FirstOrDefault(p => p.Id == policyId);
        
        var payment = new Payment
        {
            Id = _paymentIdCounter++,
            PolicyId = policyId,
            Amount = policy?.Premium ?? 0,
            Status = PaymentStatus.Pending,
            TransactionReference = $"TXN{DateTime.UtcNow:yyyyMMddHHmmss}{_paymentIdCounter}",
            CreatedAt = DateTime.UtcNow
        };

        _payments.Add(payment);
        return payment;
    }

    public Task<Payment> CompletePayment(string transactionReference)
    {
        var payment = _payments.FirstOrDefault(p => p.TransactionReference == transactionReference);
        if (payment != null)
        {
            payment.Status = PaymentStatus.Completed;
            payment.CompletedAt = DateTime.UtcNow;
        }
        return Task.FromResult(payment!);
    }

    public Task<List<Payment>> GetPolicyPayments(int policyId)
    {
        var payments = _payments
            .Where(p => p.PolicyId == policyId)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();
        return Task.FromResult(payments);
    }
}
