using Microsoft.AspNetCore.Mvc;
using UssdInsuranceService.Models;
using UssdInsuranceService.Services;

namespace UssdInsuranceService.Controllers;

/// <summary>
/// Handles USSD requests for insurance service operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UssdController : ControllerBase
{
    private readonly IUssdMenuService _menuService;
    private readonly ISessionManager _sessionManager;
    private readonly ILogger<UssdController> _logger;

    public UssdController(
        IUssdMenuService menuService,
        ISessionManager sessionManager,
        ILogger<UssdController> logger)
    {
        _menuService = menuService;
        _sessionManager = sessionManager;
        _logger = logger;
    }

    /// <summary>
    /// Processes USSD requests for insurance operations
    /// </summary>
    /// <param name="request">The USSD request containing session ID, phone number, and user input</param>
    /// <returns>A USSD response with message and session status</returns>
    /// <response code="200">Returns the USSD response message</response>
    /// <response code="500">If an internal error occurs</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/ussd
    ///     Content-Type: application/x-www-form-urlencoded
    ///     
    ///     SessionId=123456&amp;PhoneNumber=%2B254712345678&amp;Text=1&amp;ServiceCode=*123%23
    ///     
    /// Menu Flow:
    /// - 1: Register/Onboard new customer
    /// - 2: Buy Insurance policy
    /// - 3: Check Policy Status
    /// - 4: Pay Premium
    /// - 5: Renew Policy
    /// - 6: Submit Claim
    /// - 7: Check Claim Status
    /// - 8: Customer Support
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(UssdResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> HandleUssd([FromForm] UssdRequest request)
    {
        try
        {
            _logger.LogInformation($"USSD Request - SessionId: {request.SessionId}, PhoneNumber: {request.PhoneNumber}, Text: {request.Text}");

            var session = _sessionManager.GetOrCreateSession(request.SessionId, request.PhoneNumber);
            var response = await _menuService.ProcessRequest(request, session);

            if (response.EndSession)
            {
                _sessionManager.EndSession(request.SessionId);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing USSD request");
            return Ok(new UssdResponse
            {
                Message = "An error occurred. Please try again.",
                EndSession = true
            });
        }
    }
}
