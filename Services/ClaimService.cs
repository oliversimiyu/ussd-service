using UssdInsuranceService.Models;

namespace UssdInsuranceService.Services;

public class ClaimService : IClaimService
{
    private static readonly List<Claim> _claims = new();
    private static int _claimIdCounter = 1;
    private readonly IPolicyService _policyService;

    public ClaimService(IPolicyService policyService)
    {
        _policyService = policyService;
    }

    public Task<Claim> SubmitClaim(Claim claim)
    {
        claim.Id = _claimIdCounter++;
        claim.ClaimNumber = $"CLM{DateTime.UtcNow:yyyyMMdd}{claim.Id:D6}";
        claim.CreatedAt = DateTime.UtcNow;
        claim.Status = ClaimStatus.Submitted;
        _claims.Add(claim);
        return Task.FromResult(claim);
    }

    public async Task<List<Claim>> GetCustomerClaims(int customerId)
    {
        var policies = await _policyService.GetCustomerPolicies(customerId);
        var policyIds = policies.Select(p => p.Id).ToList();
        
        var claims = _claims
            .Where(c => policyIds.Contains(c.PolicyId))
            .OrderByDescending(c => c.CreatedAt)
            .ToList();
        
        return claims;
    }

    public Task<Claim?> GetClaimByNumber(string claimNumber)
    {
        var claim = _claims.FirstOrDefault(c => c.ClaimNumber == claimNumber);
        return Task.FromResult(claim);
    }
}
