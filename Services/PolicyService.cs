using UssdInsuranceService.Models;

namespace UssdInsuranceService.Services;

public class PolicyService : IPolicyService
{
    // In-memory storage for demo purposes
    private static readonly List<Customer> _customers = new();
    private static readonly List<Policy> _policies = new();
    private static int _customerIdCounter = 1;
    private static int _policyIdCounter = 1;

    public Task<Customer> RegisterCustomer(Customer customer)
    {
        customer.Id = _customerIdCounter++;
        customer.CreatedAt = DateTime.UtcNow;
        _customers.Add(customer);
        return Task.FromResult(customer);
    }

    public Task<Customer?> GetCustomerByPhone(string phoneNumber)
    {
        var customer = _customers.FirstOrDefault(c => c.PhoneNumber == phoneNumber);
        return Task.FromResult(customer);
    }

    public Task<Policy> CreatePolicy(Policy policy)
    {
        policy.Id = _policyIdCounter++;
        policy.PolicyNumber = $"POL{DateTime.UtcNow:yyyyMMdd}{policy.Id:D6}";
        policy.CreatedAt = DateTime.UtcNow;
        _policies.Add(policy);
        return Task.FromResult(policy);
    }

    public Task<List<Policy>> GetCustomerPolicies(int customerId)
    {
        var policies = _policies
            .Where(p => p.CustomerId == customerId)
            .ToList();
        return Task.FromResult(policies);
    }

    public Task<List<Policy>> GetActivePolicies(int customerId)
    {
        var policies = _policies
            .Where(p => p.CustomerId == customerId && p.Status == PolicyStatus.Active)
            .ToList();
        return Task.FromResult(policies);
    }

    public Task<List<Policy>> GetExpiredPolicies(int customerId)
    {
        var policies = _policies
            .Where(p => p.CustomerId == customerId && 
                       (p.Status == PolicyStatus.Expired || p.EndDate < DateTime.UtcNow))
            .ToList();
        return Task.FromResult(policies);
    }

    public Task<Policy> RenewPolicy(int policyId)
    {
        var policy = _policies.FirstOrDefault(p => p.Id == policyId);
        if (policy != null)
        {
            policy.Status = PolicyStatus.Active;
            policy.StartDate = DateTime.UtcNow;
            policy.EndDate = DateTime.UtcNow.AddYears(1);
            policy.NextPaymentDate = DateTime.UtcNow.AddMonths(1);
        }
        return Task.FromResult(policy!);
    }
}
