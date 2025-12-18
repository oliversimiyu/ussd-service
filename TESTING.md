# USSD Testing Guide

This guide explains how to test the USSD Insurance Service using different methods.

## Testing Methods

### 1. Interactive Python Simulator (Recommended for UI Testing)

The Python simulator provides a phone-like interface for testing the complete USSD workflow.

**Prerequisites:**
```bash
pip install requests
```

**Usage:**
```bash
# Start the API server in one terminal
dotnet run

# In another terminal, run the simulator
python3 ussd-simulator.py
```

**Features:**
- 📱 Phone-like display interface
- 🎯 Pre-configured test scenarios
- 🔄 Interactive step-by-step testing
- 🎨 Color-coded output
- 📝 Session tracking

**Test Scenarios Available:**
1. Complete Registration Flow
2. Buy Health Insurance
3. Check Policy Status
4. Customer Support
5. Custom Interactive Session

---

### 2. Bash Test Script (API Testing)

Automated API testing with curl commands.

**Usage:**
```bash
# Start the API server first
dotnet run

# In another terminal, run the test script
./test-api.sh
```

This script tests:
- ✅ Server health check
- ✅ Main menu
- ✅ Registration flow
- ✅ Insurance purchase
- ✅ Policy checking
- ✅ Support information

---

### 3. Swagger UI (API Documentation & Testing)

Interactive API documentation for manual testing.

**Access:**
1. Start the server: `dotnet run`
2. Open browser: `http://localhost:5000/swagger`
3. Expand the `/api/ussd` endpoint
4. Click "Try it out"
5. Enter test data and execute

**Example Request Body:**
```
SessionId=12345
PhoneNumber=%2B254712345678
Text=1
ServiceCode=*123%23
```

---

### 4. cURL Commands (Manual Testing)

Direct HTTP requests for fine-grained testing.

#### Main Menu
```bash
curl -X POST http://localhost:5000/api/ussd \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "SessionId=TEST123&PhoneNumber=%2B254712345678&Text=&ServiceCode=*123%23"
```

#### Registration - Start
```bash
curl -X POST http://localhost:5000/api/ussd \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "SessionId=TEST123&PhoneNumber=%2B254712345678&Text=1&ServiceCode=*123%23"
```

#### Registration - Enter Name
```bash
curl -X POST http://localhost:5000/api/ussd \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "SessionId=TEST123&PhoneNumber=%2B254712345678&Text=1*John%20Doe&ServiceCode=*123%23"
```

#### Registration - Enter ID
```bash
curl -X POST http://localhost:5000/api/ussd \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "SessionId=TEST123&PhoneNumber=%2B254712345678&Text=1*John%20Doe*12345678&ServiceCode=*123%23"
```

#### Registration - Complete
```bash
curl -X POST http://localhost:5000/api/ussd \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "SessionId=TEST123&PhoneNumber=%2B254712345678&Text=1*John%20Doe*12345678*1990-05-15&ServiceCode=*123%23"
```

---

### 5. Postman Collection (Team Testing)

For team collaboration and automated testing.

**Setup:**
1. Import the collection (create from Swagger)
2. Set environment variable: `baseUrl = http://localhost:5000`
3. Run requests in sequence

**Key Endpoints:**
- POST `/api/ussd` - Main USSD handler
- GET `/health` - Health check
- GET `/` - API info

---

## Complete Test Workflows

### Workflow 1: New Customer Registration
```
1. Dial *123# (empty Text)
   → Main menu displayed

2. Select option 1 (Text=1)
   → "Enter your full name:"

3. Enter name (Text=1*John Doe)
   → "Enter your ID number:"

4. Enter ID (Text=1*John Doe*12345678)
   → "Enter date of birth (YYYY-MM-DD):"

5. Enter DOB (Text=1*John Doe*12345678*1990-05-15)
   → "Registration successful! Welcome John Doe."
```

### Workflow 2: Buy Insurance Policy
```
1. Start new session
   → Main menu

2. Select option 2 (Buy Insurance)
   → Insurance type menu

3. Select Health Insurance (option 2)
   → Coverage amount menu

4. Select $10,000 coverage (option 1)
   → Policy created with policy number
```

### Workflow 3: Submit a Claim
```
1. Main menu → option 6
   → Select policy for claim

2. Choose policy
   → Select claim type

3. Select claim type (e.g., Medical = 1)
   → Enter description

4. Enter description
   → Claim submitted with claim number
```

---

## Testing Tips

### Session Management
- Each USSD session has a unique SessionId
- Session expires after 5 minutes of inactivity
- Use different SessionId for each test flow
- Same phone number can have multiple sessions

### Input Format
- User inputs are separated by `*` (asterisk)
- Example: `1*John Doe*12345678*1990-05-15`
- Text field accumulates all inputs in the session
- First request has empty Text field

### Response Format
```json
{
  "message": "Menu text or response",
  "endSession": false  // true = session ends, false = continue
}
```

### Phone Number Format
- Use URL encoding for + sign: `%2B`
- Example: +254712345678 → %2B254712345678
- Can use any valid phone format in testing

---

## Troubleshooting

### API Not Responding
```bash
# Check if server is running
curl http://localhost:5000/health

# If not running, start it
dotnet run
```

### Connection Refused
- Ensure the API is running on port 5000
- Check firewall settings
- Verify no other service is using port 5000

### Session Errors
- Use unique SessionId for each test
- Don't reuse SessionId after session ends
- Clear sessions by restarting the API

### Invalid Input Errors
- Check date format: YYYY-MM-DD
- Ensure numeric inputs are valid
- Verify menu option numbers exist

---

## Quick Start Commands

```bash
# Terminal 1: Start the API
cd /home/codename/ussd-insurance-service
dotnet run

# Terminal 2: Run interactive simulator
python3 ussd-simulator.py

# OR Terminal 2: Run automated tests
./test-api.sh

# OR Terminal 2: Manual curl test
curl -X POST http://localhost:5000/api/ussd \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "SessionId=TEST&PhoneNumber=%2B254712345678&Text=&ServiceCode=*123%23"
```

---

## Next Steps

1. ✅ Test all menu options
2. ✅ Verify data persistence (in-memory)
3. ✅ Test error scenarios
4. ✅ Check session timeout behavior
5. ✅ Test concurrent sessions
6. 🔄 Integrate with real USSD gateway
7. 🔄 Add database persistence
8. 🔄 Implement payment gateway

---

## Additional Resources

- **API Documentation**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/health
- **API Info**: http://localhost:5000/
- **README**: See README.md for full documentation
