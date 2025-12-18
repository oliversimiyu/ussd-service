#!/bin/bash

# USSD Insurance Service API Test Script
# This script tests the main USSD endpoints

BASE_URL="http://localhost:5000"
PHONE_NUMBER="%2B254712345678"
SESSION_ID="TEST$(date +%s)"

echo "================================================"
echo "USSD Insurance Service API Test"
echo "================================================"
echo ""

# Color codes for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Function to make USSD request
make_ussd_request() {
    local text="$1"
    local description="$2"
    
    echo -e "${BLUE}Testing: $description${NC}"
    echo "Input: $text"
    
    response=$(curl -s -X POST "$BASE_URL/api/ussd" \
        -H "Content-Type: application/x-www-form-urlencoded" \
        -d "SessionId=$SESSION_ID&PhoneNumber=$PHONE_NUMBER&Text=$text&ServiceCode=*123%23")
    
    echo -e "${GREEN}Response:${NC}"
    echo "$response" | jq '.' 2>/dev/null || echo "$response"
    echo ""
    echo "---"
    echo ""
}

# Check if server is running
echo "Checking if server is running..."
health_check=$(curl -s "$BASE_URL/health")
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✓ Server is running${NC}"
    echo "$health_check" | jq '.' 2>/dev/null
    echo ""
else
    echo -e "${RED}✗ Server is not running. Please start it with: dotnet run${NC}"
    exit 1
fi

echo "================================================"
echo "Running USSD Flow Tests"
echo "================================================"
echo ""

# Test 1: Main Menu
make_ussd_request "" "Main Menu"

# Test 2: Registration Flow - Start
make_ussd_request "1" "Registration - Start"

# Test 3: Registration - Name
make_ussd_request "1*John%20Doe" "Registration - Enter Name"

# Test 4: Registration - ID Number
make_ussd_request "1*John%20Doe*12345678" "Registration - Enter ID"

# Test 5: Registration - Complete
make_ussd_request "1*John%20Doe*12345678*1990-05-15" "Registration - Complete"

# Test 6: Buy Insurance Menu
SESSION_ID="TEST$(date +%s)"  # New session
make_ussd_request "2" "Buy Insurance - Menu"

# Test 7: Select Health Insurance
make_ussd_request "2*2" "Buy Insurance - Select Health"

# Test 8: Select Coverage
make_ussd_request "2*2*1" "Buy Insurance - Select Coverage & Complete"

# Test 9: Check Policy
SESSION_ID="TEST$(date +%s)"  # New session
make_ussd_request "3" "Check Policy Status"

# Test 10: Customer Support
SESSION_ID="TEST$(date +%s)"  # New session
make_ussd_request "8" "Customer Support"

echo "================================================"
echo "Testing Additional Endpoints"
echo "================================================"
echo ""

# Test API Info
echo -e "${BLUE}Testing: API Info Endpoint${NC}"
curl -s "$BASE_URL/" | jq '.'
echo ""
echo "---"
echo ""

# Test Swagger
echo -e "${BLUE}Testing: Swagger Endpoint${NC}"
swagger_response=$(curl -s -o /dev/null -w "%{http_code}" "$BASE_URL/swagger/index.html")
if [ "$swagger_response" = "200" ]; then
    echo -e "${GREEN}✓ Swagger UI is accessible at $BASE_URL/swagger${NC}"
else
    echo -e "${RED}✗ Swagger UI is not accessible${NC}"
fi
echo ""

echo "================================================"
echo "Test Complete!"
echo "================================================"
echo ""
echo "To view full API documentation, visit:"
echo "  $BASE_URL/swagger"
echo ""
echo "To test manually, use:"
echo "  curl -X POST $BASE_URL/api/ussd \\"
echo "    -H \"Content-Type: application/x-www-form-urlencoded\" \\"
echo "    -d \"SessionId=12345&PhoneNumber=$PHONE_NUMBER&Text=&ServiceCode=*123%23\""
echo ""
