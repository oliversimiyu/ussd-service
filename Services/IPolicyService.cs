using UssdInsuranceService.Models;

namespace UssdInsuranceService.Services;

public interface IPolicyService
{
    Task<Customer> RegisterCustomer(Customer customer);
    Task<Customer?> GetCustomerByPhone(string phoneNumber);
    Task<Policy> CreatePolicy(Policy policy);
    Task<List<Policy>> GetCustomerPolicies(int customerId);
    Task<List<Policy>> GetActivePolicies(int customerId);
    Task<List<Policy>> GetExpiredPolicies(int customerId);
    Task<Policy> RenewPolicy(int policyId);
}
