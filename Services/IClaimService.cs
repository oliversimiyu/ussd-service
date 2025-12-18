using UssdInsuranceService.Models;

namespace UssdInsuranceService.Services;

public interface IClaimService
{
    Task<Claim> SubmitClaim(Claim claim);
    Task<List<Claim>> GetCustomerClaims(int customerId);
    Task<Claim?> GetClaimByNumber(string claimNumber);
}
