using UssdInsuranceService.Models;

namespace UssdInsuranceService.Services;

public interface IPaymentService
{
    Task<Payment> InitiatePayment(int policyId);
    Task<Payment> CompletePayment(string transactionReference);
    Task<List<Payment>> GetPolicyPayments(int policyId);
}
