using UssdInsuranceService.Models;

namespace UssdInsuranceService.Services;

public class UssdMenuService : IUssdMenuService
{
    private readonly IPolicyService _policyService;
    private readonly IClaimService _claimService;
    private readonly IPaymentService _paymentService;
    private readonly ISessionManager _sessionManager;

    public UssdMenuService(
        IPolicyService policyService,
        IClaimService claimService,
        IPaymentService paymentService,
        ISessionManager sessionManager)
    {
        _policyService = policyService;
        _claimService = claimService;
        _paymentService = paymentService;
        _sessionManager = sessionManager;
    }

    public async Task<UssdResponse> ProcessRequest(UssdRequest request, UssdSession session)
    {
        var inputs = request.Text.Split('*');
        var currentInput = inputs.Length > 0 ? inputs[^1] : "";

        return session.CurrentMenu switch
        {
            "main" => MainMenu(),
            "register" => await HandleRegistration(currentInput, session),
            "buy_insurance" => await HandleBuyInsurance(currentInput, session),
            "check_policy" => await HandleCheckPolicy(session),
            "pay_premium" => await HandlePayPremium(currentInput, session),
            "renew_policy" => await HandleRenewPolicy(currentInput, session),
            "submit_claim" => await HandleSubmitClaim(currentInput, session),
            "check_claim" => await HandleCheckClaim(session),
            "support" => HandleSupport(currentInput),
            _ => MainMenu()
        };
    }

    private UssdResponse MainMenu()
    {
        return new UssdResponse
        {
            Message = "Welcome to Insurance Services\n" +
                     "1. Register/Onboard\n" +
                     "2. Buy Insurance\n" +
                     "3. Check Policy Status\n" +
                     "4. Pay Premium\n" +
                     "5. Renew Policy\n" +
                     "6. Submit Claim\n" +
                     "7. Check Claim Status\n" +
                     "8. Customer Support",
            EndSession = false
        };
    }

    private async Task<UssdResponse> HandleRegistration(string input, UssdSession session)
    {
        if (!session.Data.ContainsKey("step"))
        {
            session.Data["step"] = "name";
            session.CurrentMenu = "register";
            _sessionManager.UpdateSession(session);
            return new UssdResponse
            {
                Message = "Registration\nEnter your full name:",
                EndSession = false
            };
        }

        var step = session.Data["step"];

        switch (step)
        {
            case "name":
                session.Data["name"] = input;
                session.Data["step"] = "id_number";
                _sessionManager.UpdateSession(session);
                return new UssdResponse
                {
                    Message = "Enter your ID number:",
                    EndSession = false
                };

            case "id_number":
                session.Data["id_number"] = input;
                session.Data["step"] = "dob";
                _sessionManager.UpdateSession(session);
                return new UssdResponse
                {
                    Message = "Enter date of birth (YYYY-MM-DD):",
                    EndSession = false
                };

            case "dob":
                session.Data["dob"] = input;
                
                var customer = new Customer
                {
                    PhoneNumber = session.PhoneNumber,
                    Name = session.Data["name"],
                    IdNumber = session.Data["id_number"],
                    DateOfBirth = DateTime.Parse(input)
                };

                await _policyService.RegisterCustomer(customer);

                return new UssdResponse
                {
                    Message = $"Registration successful!\nWelcome {customer.Name}.\nYou can now buy insurance policies.",
                    EndSession = true
                };

            default:
                return MainMenu();
        }
    }

    private async Task<UssdResponse> HandleBuyInsurance(string input, UssdSession session)
    {
        if (!session.Data.ContainsKey("step"))
        {
            session.Data["step"] = "select_type";
            session.CurrentMenu = "buy_insurance";
            _sessionManager.UpdateSession(session);
            return new UssdResponse
            {
                Message = "Select Insurance Type:\n" +
                         "1. Micro Insurance\n" +
                         "2. Health Insurance\n" +
                         "3. Motor Insurance\n" +
                         "4. Funeral Cover\n" +
                         "5. Life Insurance",
                EndSession = false
            };
        }

        var step = session.Data["step"];

        switch (step)
        {
            case "select_type":
                var typeMapping = new Dictionary<string, PolicyType>
                {
                    {"1", PolicyType.MicroInsurance},
                    {"2", PolicyType.Health},
                    {"3", PolicyType.Motor},
                    {"4", PolicyType.Funeral},
                    {"5", PolicyType.Life}
                };

                if (!typeMapping.ContainsKey(input))
                {
                    return new UssdResponse
                    {
                        Message = "Invalid selection. Please try again.",
                        EndSession = true
                    };
                }

                session.Data["policy_type"] = input;
                session.Data["step"] = "select_cover";
                _sessionManager.UpdateSession(session);

                var coverOptions = GetCoverOptions(typeMapping[input]);
                return new UssdResponse
                {
                    Message = $"Select Cover Amount:\n{coverOptions}",
                    EndSession = false
                };

            case "select_cover":
                session.Data["cover_amount"] = input;
                
                var selectedType = session.Data["policy_type"];
                var policyType = new Dictionary<string, PolicyType>
                {
                    {"1", PolicyType.MicroInsurance},
                    {"2", PolicyType.Health},
                    {"3", PolicyType.Motor},
                    {"4", PolicyType.Funeral},
                    {"5", PolicyType.Life}
                }[selectedType];

                var premium = CalculatePremium(policyType, int.Parse(input));
                var customer = await _policyService.GetCustomerByPhone(session.PhoneNumber);

                if (customer == null)
                {
                    return new UssdResponse
                    {
                        Message = "Please register first (option 1 from main menu)",
                        EndSession = true
                    };
                }

                var policy = await _policyService.CreatePolicy(new Policy
                {
                    CustomerId = customer.Id,
                    Type = policyType,
                    Status = PolicyStatus.Pending,
                    Premium = premium,
                    CoverAmount = GetCoverAmountValue(int.Parse(input)),
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddYears(1),
                    NextPaymentDate = DateTime.UtcNow.AddMonths(1)
                });

                return new UssdResponse
                {
                    Message = $"Policy Created!\n" +
                             $"Policy No: {policy.PolicyNumber}\n" +
                             $"Premium: ${premium}/month\n" +
                             $"Cover: ${policy.CoverAmount}\n" +
                             $"Complete payment to activate.",
                    EndSession = true
                };

            default:
                return MainMenu();
        }
    }

    private async Task<UssdResponse> HandleCheckPolicy(UssdSession session)
    {
        var customer = await _policyService.GetCustomerByPhone(session.PhoneNumber);
        
        if (customer == null)
        {
            return new UssdResponse
            {
                Message = "No customer account found. Please register first.",
                EndSession = true
            };
        }

        var policies = await _policyService.GetCustomerPolicies(customer.Id);

        if (!policies.Any())
        {
            return new UssdResponse
            {
                Message = "You have no active policies.",
                EndSession = true
            };
        }

        var message = "Your Policies:\n";
        foreach (var policy in policies)
        {
            message += $"\n{policy.PolicyNumber}\n" +
                      $"Type: {policy.Type}\n" +
                      $"Status: {policy.Status}\n" +
                      $"Premium: ${policy.Premium}\n" +
                      $"Next Payment: {policy.NextPaymentDate:yyyy-MM-dd}\n";
        }

        return new UssdResponse
        {
            Message = message,
            EndSession = true
        };
    }

    private async Task<UssdResponse> HandlePayPremium(string input, UssdSession session)
    {
        if (!session.Data.ContainsKey("step"))
        {
            var customer = await _policyService.GetCustomerByPhone(session.PhoneNumber);
            if (customer == null)
            {
                return new UssdResponse
                {
                    Message = "Please register first.",
                    EndSession = true
                };
            }

            var policies = await _policyService.GetCustomerPolicies(customer.Id);
            if (!policies.Any())
            {
                return new UssdResponse
                {
                    Message = "No policies found.",
                    EndSession = true
                };
            }

            var message = "Select policy to pay:\n";
            for (int i = 0; i < policies.Count; i++)
            {
                message += $"{i + 1}. {policies[i].PolicyNumber} - ${policies[i].Premium}\n";
            }

            session.Data["step"] = "select_policy";
            session.Data["policies"] = string.Join(",", policies.Select(p => p.Id));
            session.CurrentMenu = "pay_premium";
            _sessionManager.UpdateSession(session);

            return new UssdResponse
            {
                Message = message,
                EndSession = false
            };
        }

        var policyIds = session.Data["policies"].Split(',').Select(int.Parse).ToList();
        var selectedIndex = int.Parse(input) - 1;

        if (selectedIndex < 0 || selectedIndex >= policyIds.Count)
        {
            return new UssdResponse
            {
                Message = "Invalid selection.",
                EndSession = true
            };
        }

        var payment = await _paymentService.InitiatePayment(policyIds[selectedIndex]);

        return new UssdResponse
        {
            Message = $"Payment initiated!\n" +
                     $"Amount: ${payment.Amount}\n" +
                     $"Reference: {payment.TransactionReference}\n" +
                     $"Complete payment via M-Pesa to activate.",
            EndSession = true
        };
    }

    private async Task<UssdResponse> HandleRenewPolicy(string input, UssdSession session)
    {
        if (!session.Data.ContainsKey("step"))
        {
            var customer = await _policyService.GetCustomerByPhone(session.PhoneNumber);
            if (customer == null)
            {
                return new UssdResponse
                {
                    Message = "Please register first.",
                    EndSession = true
                };
            }

            var policies = await _policyService.GetExpiredPolicies(customer.Id);
            if (!policies.Any())
            {
                return new UssdResponse
                {
                    Message = "No policies need renewal.",
                    EndSession = true
                };
            }

            var message = "Select policy to renew:\n";
            for (int i = 0; i < policies.Count; i++)
            {
                message += $"{i + 1}. {policies[i].PolicyNumber} - {policies[i].Type}\n";
            }

            session.Data["step"] = "select_policy";
            session.Data["policies"] = string.Join(",", policies.Select(p => p.Id));
            session.CurrentMenu = "renew_policy";
            _sessionManager.UpdateSession(session);

            return new UssdResponse
            {
                Message = message,
                EndSession = false
            };
        }

        var policyIds = session.Data["policies"].Split(',').Select(int.Parse).ToList();
        var selectedIndex = int.Parse(input) - 1;

        if (selectedIndex < 0 || selectedIndex >= policyIds.Count)
        {
            return new UssdResponse
            {
                Message = "Invalid selection.",
                EndSession = true
            };
        }

        var renewed = await _policyService.RenewPolicy(policyIds[selectedIndex]);

        return new UssdResponse
        {
            Message = $"Policy renewed successfully!\n" +
                     $"Policy No: {renewed.PolicyNumber}\n" +
                     $"New End Date: {renewed.EndDate:yyyy-MM-dd}",
            EndSession = true
        };
    }

    private async Task<UssdResponse> HandleSubmitClaim(string input, UssdSession session)
    {
        if (!session.Data.ContainsKey("step"))
        {
            var customer = await _policyService.GetCustomerByPhone(session.PhoneNumber);
            if (customer == null)
            {
                return new UssdResponse
                {
                    Message = "Please register first.",
                    EndSession = true
                };
            }

            var policies = await _policyService.GetActivePolicies(customer.Id);
            if (!policies.Any())
            {
                return new UssdResponse
                {
                    Message = "No active policies found.",
                    EndSession = true
                };
            }

            var message = "Select policy for claim:\n";
            for (int i = 0; i < policies.Count; i++)
            {
                message += $"{i + 1}. {policies[i].PolicyNumber} - {policies[i].Type}\n";
            }

            session.Data["step"] = "select_policy";
            session.Data["policies"] = string.Join(",", policies.Select(p => p.Id));
            session.CurrentMenu = "submit_claim";
            _sessionManager.UpdateSession(session);

            return new UssdResponse
            {
                Message = message,
                EndSession = false
            };
        }

        var step = session.Data["step"];

        switch (step)
        {
            case "select_policy":
                session.Data["selected_policy"] = input;
                session.Data["step"] = "claim_type";
                _sessionManager.UpdateSession(session);
                return new UssdResponse
                {
                    Message = "Claim Type:\n" +
                             "1. Medical\n" +
                             "2. Accident\n" +
                             "3. Death\n" +
                             "4. Theft\n" +
                             "5. Damage",
                    EndSession = false
                };

            case "claim_type":
                session.Data["claim_type"] = input;
                session.Data["step"] = "description";
                _sessionManager.UpdateSession(session);
                return new UssdResponse
                {
                    Message = "Brief description of incident:",
                    EndSession = false
                };

            case "description":
                var policyIds = session.Data["policies"].Split(',').Select(int.Parse).ToList();
                var selectedIndex = int.Parse(session.Data["selected_policy"]) - 1;
                var policyId = policyIds[selectedIndex];

                var claimType = session.Data["claim_type"] switch
                {
                    "1" => ClaimType.Medical,
                    "2" => ClaimType.Accident,
                    "3" => ClaimType.Death,
                    "4" => ClaimType.Theft,
                    "5" => ClaimType.Damage,
                    _ => ClaimType.Other
                };

                var claim = await _claimService.SubmitClaim(new Claim
                {
                    PolicyId = policyId,
                    Type = claimType,
                    Description = input,
                    IncidentDate = DateTime.UtcNow,
                    Status = ClaimStatus.Submitted
                });

                return new UssdResponse
                {
                    Message = $"Claim submitted successfully!\n" +
                             $"Claim No: {claim.ClaimNumber}\n" +
                             $"Status: Under Review\n" +
                             $"You'll be contacted within 48 hours.",
                    EndSession = true
                };

            default:
                return MainMenu();
        }
    }

    private async Task<UssdResponse> HandleCheckClaim(UssdSession session)
    {
        var customer = await _policyService.GetCustomerByPhone(session.PhoneNumber);
        
        if (customer == null)
        {
            return new UssdResponse
            {
                Message = "No customer account found.",
                EndSession = true
            };
        }

        var claims = await _claimService.GetCustomerClaims(customer.Id);

        if (!claims.Any())
        {
            return new UssdResponse
            {
                Message = "No claims found.",
                EndSession = true
            };
        }

        var message = "Your Claims:\n";
        foreach (var claim in claims)
        {
            message += $"\n{claim.ClaimNumber}\n" +
                      $"Type: {claim.Type}\n" +
                      $"Status: {claim.Status}\n" +
                      $"Amount: ${claim.Amount}\n" +
                      $"Date: {claim.CreatedAt:yyyy-MM-dd}\n";
        }

        return new UssdResponse
        {
            Message = message,
            EndSession = true
        };
    }

    private UssdResponse HandleSupport(string input)
    {
        return new UssdResponse
        {
            Message = "Customer Support:\n" +
                     "Call: 0800-123-456\n" +
                     "Email: support@insurance.com\n" +
                     "Hours: Mon-Fri 8AM-6PM\n\n" +
                     "FAQs:\n" +
                     "1. How to claim?\n" +
                     "2. Payment methods?\n" +
                     "3. Coverage details?",
            EndSession = true
        };
    }

    private string GetCoverOptions(PolicyType type)
    {
        return type switch
        {
            PolicyType.MicroInsurance => "1. $1,000\n2. $2,500\n3. $5,000",
            PolicyType.Health => "1. $10,000\n2. $25,000\n3. $50,000",
            PolicyType.Motor => "1. $15,000\n2. $30,000\n3. $50,000",
            PolicyType.Funeral => "1. $3,000\n2. $5,000\n3. $10,000",
            PolicyType.Life => "1. $50,000\n2. $100,000\n3. $250,000",
            _ => "1. $5,000\n2. $10,000\n3. $25,000"
        };
    }

    private decimal CalculatePremium(PolicyType type, int coverOption)
    {
        var basePremium = type switch
        {
            PolicyType.MicroInsurance => coverOption * 5,
            PolicyType.Health => coverOption * 50,
            PolicyType.Motor => coverOption * 75,
            PolicyType.Funeral => coverOption * 15,
            PolicyType.Life => coverOption * 100,
            _ => coverOption * 25
        };

        return basePremium;
    }

    private decimal GetCoverAmountValue(int option)
    {
        return option switch
        {
            1 => 10000,
            2 => 25000,
            3 => 50000,
            _ => 10000
        };
    }
}
