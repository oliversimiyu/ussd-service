using UssdInsuranceService.Models;

namespace UssdInsuranceService.Services;

public interface IUssdMenuService
{
    Task<UssdResponse> ProcessRequest(UssdRequest request, UssdSession session);
}
