# USSD Insurance Service

A comprehensive USSD-based insurance management system built with ASP.NET Core 8.0. This service enables users to manage insurance policies, submit claims, make payments, and access customer support through a simple USSD interface.

## 🚀 Features

### Core Functionality
- **Customer Registration**: Onboard new customers with personal details
- **Policy Management**: 
  - Buy various insurance types (Micro, Health, Motor, Funeral, Life)
  - Check policy status
  - Renew expired policies
- **Claims Processing**: 
  - Submit new claims
  - Track claim status
  - Multiple claim types (Medical, Accident, Death, Theft, Damage)
- **Payment Processing**: 
  - Initiate premium payments
  - Track payment history
  - Integration-ready for mobile money platforms
- **Customer Support**: Access to help resources and contact information

### Insurance Types
1. **Micro Insurance** - $1,000 to $5,000 cover
2. **Health Insurance** - $10,000 to $50,000 cover
3. **Motor Insurance** - $15,000 to $50,000 cover
4. **Funeral Cover** - $3,000 to $10,000 cover
5. **Life Insurance** - $50,000 to $250,000 cover

## 🏗️ Architecture

### Technology Stack
- **Framework**: ASP.NET Core 8.0
- **Language**: C# 12
- **API Style**: RESTful
- **Documentation**: Swagger/OpenAPI
- **Storage**: In-memory (demo purposes)

### Project Structure
```
ussd-insurance-service/
├── Controllers/
│   └── UssdController.cs          # Main USSD endpoint handler
├── Models/
│   ├── InsuranceModels.cs         # Domain models (Customer, Policy, Claim, Payment)
│   └── UssdModels.cs              # USSD-specific models
├── Services/
│   ├── IUssdMenuService.cs        # USSD menu navigation interface
│   ├── UssdMenuService.cs         # Main USSD logic implementation
│   ├── IPolicyService.cs          # Policy management interface
│   ├── PolicyService.cs           # Policy operations
│   ├── IClaimService.cs           # Claims processing interface
│   ├── ClaimService.cs            # Claims operations
│   ├── IPaymentService.cs         # Payment processing interface
│   ├── PaymentService.cs          # Payment operations
│   ├── ISessionManager.cs         # Session management interface
│   └── SessionManager.cs          # USSD session handling
├── Program.cs                     # Application startup
├── appsettings.json              # Configuration
└── UssdInsuranceService.csproj   # Project file
```

## 📋 Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A code editor (Visual Studio 2022, VS Code, or JetBrains Rider)
- Optional: Docker for containerized deployment

## 🛠️ Installation & Setup

### 1. Clone the Repository
```bash
git clone <repository-url>
cd ussd-insurance-service
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Build the Project
```bash
dotnet build
```

### 4. Run the Application
```bash
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `http://localhost:5000/swagger`

## 📡 API Documentation

### USSD Endpoint

**POST** `/api/ussd`

Handles USSD requests with form-encoded data.

#### Request Parameters
```
SessionId: string       # Unique session identifier
PhoneNumber: string     # User's phone number
Text: string           # USSD input (menu selections separated by *)
ServiceCode: string    # USSD service code (e.g., *123#)
```

#### Response Format
```json
{
  "message": "Welcome to Insurance Services\n1. Register...",
  "endSession": false
}
```

#### Response Types
- `CON {message}` - Continue session (user input required)
- `END {message}` - End session (final message)

### USSD Menu Flow

```
Main Menu
├── 1. Register/Onboard
│   ├── Enter full name
│   ├── Enter ID number
│   └── Enter date of birth (YYYY-MM-DD)
├── 2. Buy Insurance
│   ├── Select insurance type
│   ├── Select cover amount
│   └── Confirm policy creation
├── 3. Check Policy Status
│   └── Display all customer policies
├── 4. Pay Premium
│   ├── Select policy
│   └── Initiate payment
├── 5. Renew Policy
│   ├── Select expired policy
│   └── Confirm renewal
├── 6. Submit Claim
│   ├── Select policy
│   ├── Select claim type
│   └── Enter description
├── 7. Check Claim Status
│   └── Display all customer claims
└── 8. Customer Support
    └── Display contact information
```

## 💻 Usage Examples

### Example 1: New Customer Registration
```
Request: *123#
Response: CON Welcome to Insurance Services
          1. Register/Onboard
          2. Buy Insurance
          ...

User Input: 1
Response: CON Registration
          Enter your full name:

User Input: John Doe
Response: CON Enter your ID number:

User Input: 12345678
Response: CON Enter date of birth (YYYY-MM-DD):

User Input: 1990-05-15
Response: END Registration successful!
          Welcome John Doe.
          You can now buy insurance policies.
```

### Example 2: Buying Insurance
```
Request: *123#
User Input: 2
Response: CON Select Insurance Type:
          1. Micro Insurance
          2. Health Insurance
          3. Motor Insurance
          4. Funeral Cover
          5. Life Insurance

User Input: 2
Response: CON Select Cover Amount:
          1. $10,000
          2. $25,000
          3. $50,000

User Input: 1
Response: END Policy Created!
          Policy No: POL2025121900001
          Premium: $50/month
          Cover: $10000
          Complete payment to activate.
```

### Example 3: Testing with cURL
```bash
# Main menu request
curl -X POST http://localhost:5000/api/ussd \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "SessionId=123456&PhoneNumber=%2B254712345678&Text=&ServiceCode=*123%23"

# Registration flow
curl -X POST http://localhost:5000/api/ussd \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "SessionId=123456&PhoneNumber=%2B254712345678&Text=1*John%20Doe&ServiceCode=*123%23"
```

## 🔧 Configuration

### appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=InsuranceDb;User Id=sa;Password=YourPassword123;"
  }
}
```

## 🏃‍♂️ Development

### Running Tests
```bash
dotnet test
```

### Watch Mode (Auto-rebuild)
```bash
dotnet watch run
```

### Publishing for Production
```bash
dotnet publish -c Release -o ./publish
```

## 🔐 Security Considerations

⚠️ **Important**: This is a demonstration project with in-memory storage. For production use:

1. Implement persistent database storage (SQL Server, PostgreSQL, etc.)
2. Add authentication and authorization
3. Implement rate limiting
4. Add input validation and sanitization
5. Secure sensitive configuration with environment variables
6. Implement proper error handling and logging
7. Add encryption for sensitive data
8. Implement secure payment gateway integration
9. Add HTTPS enforcement
10. Regular security audits

## 🚢 Production Deployment Checklist

- [ ] Replace in-memory storage with database
- [ ] Configure production logging (Application Insights, Serilog, etc.)
- [ ] Set up CI/CD pipeline
- [ ] Configure environment variables
- [ ] Implement health checks
- [ ] Set up monitoring and alerting
- [ ] Configure backup strategies
- [ ] Implement payment gateway integration
- [ ] Add SMS notification service
- [ ] Set up load balancing
- [ ] Configure CORS policies
- [ ] Implement API versioning
- [ ] Add comprehensive error handling

## 📊 Data Models

### Customer
- Id, PhoneNumber, Name, IdNumber, DateOfBirth, Email, CreatedAt

### Policy
- Id, CustomerId, PolicyNumber, Type, Status, Premium, CoverAmount
- StartDate, EndDate, NextPaymentDate, CreatedAt

### Claim
- Id, PolicyId, ClaimNumber, Type, Status, Amount, Description
- IncidentDate, CreatedAt, ProcessedAt

### Payment
- Id, PolicyId, Amount, Status, TransactionReference
- CreatedAt, CompletedAt

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📞 Support

For questions or support:
- Email: oliverwafula2020@gmail.com
- Phone: +254710500108
- Hours: Mon-Fri 8AM-6PM

## 🗺️ Roadmap

- [ ] Add database persistence (Entity Framework Core)
- [ ] Implement unit and integration tests
- [ ] Add payment gateway integration (M-Pesa, Stripe)
- [ ] SMS notifications
- [ ] Admin dashboard
- [ ] Real-time claim processing
- [ ] Document upload functionality
- [ ] Multi-language support
- [ ] Analytics and reporting
- [ ] Mobile app integration

## 🙏 Acknowledgments

Built with ASP.NET Core and designed for USSD-based insurance services in emerging markets.
