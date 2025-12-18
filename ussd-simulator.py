#!/usr/bin/env python3
"""
Interactive USSD Insurance Service Simulator
This script simulates a USSD interface for testing the insurance service
"""

import requests
import sys
import time
import random
from datetime import datetime

# Configuration
API_URL = "http://localhost:5000/api/ussd"
PHONE_NUMBER = "+254712345678"

# ANSI color codes
class Colors:
    HEADER = '\033[95m'
    BLUE = '\033[94m'
    CYAN = '\033[96m'
    GREEN = '\033[92m'
    YELLOW = '\033[93m'
    RED = '\033[91m'
    END = '\033[0m'
    BOLD = '\033[1m'
    UNDERLINE = '\033[4m'

def clear_screen():
    """Clear the terminal screen"""
    print("\033[2J\033[H", end="")

def print_header():
    """Print the USSD header"""
    clear_screen()
    print(f"{Colors.BOLD}{Colors.CYAN}{'=' * 60}{Colors.END}")
    print(f"{Colors.BOLD}{Colors.CYAN}  USSD INSURANCE SERVICE - INTERACTIVE SIMULATOR{Colors.END}")
    print(f"{Colors.BOLD}{Colors.CYAN}{'=' * 60}{Colors.END}\n")

def print_phone_display(message, end_session=False):
    """Display message in a phone-like format"""
    lines = message.split('\n')
    max_width = max(len(line) for line in lines) + 4
    max_width = max(max_width, 40)
    
    print(f"\n{Colors.BOLD}┌{'─' * (max_width - 2)}┐{Colors.END}")
    for line in lines:
        padding = max_width - len(line) - 4
        print(f"{Colors.BOLD}│{Colors.END} {Colors.GREEN}{line}{Colors.END}{' ' * padding} {Colors.BOLD}│{Colors.END}")
    print(f"{Colors.BOLD}└{'─' * (max_width - 2)}┘{Colors.END}\n")
    
    if end_session:
        print(f"{Colors.YELLOW}[Session Ended]{Colors.END}\n")
    else:
        print(f"{Colors.CYAN}[Waiting for input...]{Colors.END}\n")

def make_ussd_request(text, session_id, phone_number):
    """Make a USSD API request"""
    try:
        data = {
            'SessionId': session_id,
            'PhoneNumber': phone_number,
            'Text': text,
            'ServiceCode': '*123#'
        }
        
        response = requests.post(API_URL, data=data)
        
        if response.status_code == 200:
            return response.json()
        else:
            return {
                'message': f"Error: Server returned status {response.status_code}",
                'endSession': True
            }
    except requests.exceptions.ConnectionError:
        return {
            'message': "Error: Cannot connect to server. Please ensure the API is running.",
            'endSession': True
        }
    except Exception as e:
        return {
            'message': f"Error: {str(e)}",
            'endSession': True
        }

def simulate_ussd_session(phone_number):
    """Simulate a complete USSD session"""
    session_id = f"SIM_{int(datetime.now().timestamp())}_{random.randint(1000, 9999)}"
    user_input_history = []
    
    print_header()
    print(f"{Colors.BOLD}Phone Number:{Colors.END} {Colors.GREEN}{phone_number}{Colors.END}")
    print(f"{Colors.BOLD}Session ID:{Colors.END} {Colors.YELLOW}{session_id}{Colors.END}\n")
    print(f"{Colors.CYAN}Dialing *123#...{Colors.END}\n")
    time.sleep(1)
    
    # Initial request (main menu)
    response = make_ussd_request("", session_id, phone_number)
    
    while True:
        print_header()
        print(f"{Colors.BOLD}Phone:{Colors.END} {phone_number}  {Colors.BOLD}Session:{Colors.END} {session_id[:20]}...\n")
        
        if user_input_history:
            print(f"{Colors.BOLD}Previous inputs:{Colors.END} {Colors.YELLOW}{' > '.join(user_input_history)}{Colors.END}\n")
        
        print_phone_display(response['message'], response['endSession'])
        
        if response['endSession']:
            print(f"\n{Colors.GREEN}{'─' * 60}{Colors.END}")
            print(f"{Colors.BOLD}Session completed!{Colors.END}")
            print(f"{Colors.GREEN}{'─' * 60}{Colors.END}\n")
            
            restart = input(f"{Colors.CYAN}Start a new session? (y/n): {Colors.END}").strip().lower()
            if restart == 'y':
                simulate_ussd_session(phone_number)
            return
        
        # Get user input
        user_input = input(f"{Colors.BOLD}Enter your choice (or 'q' to quit): {Colors.END}").strip()
        
        if user_input.lower() == 'q':
            print(f"\n{Colors.YELLOW}Exiting simulator...{Colors.END}\n")
            return
        
        if not user_input:
            print(f"{Colors.RED}Invalid input. Please try again.{Colors.END}")
            time.sleep(1)
            continue
        
        # Build the text parameter (accumulated inputs separated by *)
        user_input_history.append(user_input)
        text = '*'.join(user_input_history)
        
        # Make request
        response = make_ussd_request(text, session_id, phone_number)
        time.sleep(0.5)  # Simulate network delay

def show_quick_test_menu():
    """Show menu for pre-configured test scenarios"""
    scenarios = [
        {
            'name': 'Complete Registration Flow',
            'description': 'Test customer registration from start to finish',
            'steps': ['1', 'John Doe', '12345678', '1990-05-15']
        },
        {
            'name': 'Buy Health Insurance',
            'description': 'Register and purchase health insurance',
            'steps': ['1', 'Jane Smith', '87654321', '1985-03-20']
        },
        {
            'name': 'Check Policy Status',
            'description': 'View existing policies',
            'steps': ['3']
        },
        {
            'name': 'Customer Support',
            'description': 'View support information',
            'steps': ['8']
        }
    ]
    
    print_header()
    print(f"{Colors.BOLD}QUICK TEST SCENARIOS{Colors.END}\n")
    
    for i, scenario in enumerate(scenarios, 1):
        print(f"{Colors.GREEN}{i}.{Colors.END} {Colors.BOLD}{scenario['name']}{Colors.END}")
        print(f"   {scenario['description']}\n")
    
    print(f"{Colors.GREEN}0.{Colors.END} {Colors.BOLD}Custom Interactive Session{Colors.END}")
    print(f"   Manually enter inputs step by step\n")
    
    choice = input(f"{Colors.CYAN}Select scenario (0-{len(scenarios)}) or 'q' to quit: {Colors.END}").strip()
    
    if choice.lower() == 'q':
        return None
    
    try:
        choice_num = int(choice)
        if choice_num == 0:
            return 'interactive'
        elif 1 <= choice_num <= len(scenarios):
            return scenarios[choice_num - 1]
    except ValueError:
        pass
    
    print(f"{Colors.RED}Invalid choice. Please try again.{Colors.END}")
    time.sleep(1)
    return show_quick_test_menu()

def run_automated_scenario(scenario, phone_number):
    """Run a pre-configured test scenario automatically"""
    session_id = f"AUTO_{int(datetime.now().timestamp())}_{random.randint(1000, 9999)}"
    
    print_header()
    print(f"{Colors.BOLD}{Colors.GREEN}Running: {scenario['name']}{Colors.END}")
    print(f"{scenario['description']}\n")
    print(f"{Colors.BOLD}Phone:{Colors.END} {phone_number}")
    print(f"{Colors.BOLD}Session:{Colors.END} {session_id}\n")
    
    input(f"{Colors.CYAN}Press Enter to start...{Colors.END}")
    
    # Initial request
    response = make_ussd_request("", session_id, phone_number)
    user_input_history = []
    
    for step in scenario['steps']:
        print_header()
        print(f"{Colors.BOLD}{Colors.GREEN}Running: {scenario['name']}{Colors.END}\n")
        print_phone_display(response['message'], response['endSession'])
        
        print(f"{Colors.YELLOW}→ Auto-entering: {step}{Colors.END}\n")
        time.sleep(1.5)
        
        user_input_history.append(step)
        text = '*'.join(user_input_history)
        response = make_ussd_request(text, session_id, phone_number)
        
        if response['endSession']:
            break
    
    # Show final response
    print_header()
    print(f"{Colors.BOLD}{Colors.GREEN}Scenario Complete: {scenario['name']}{Colors.END}\n")
    print_phone_display(response['message'], response['endSession'])
    
    input(f"\n{Colors.CYAN}Press Enter to continue...{Colors.END}")

def check_api_status():
    """Check if the API is running"""
    try:
        response = requests.get("http://localhost:5000/health", timeout=2)
        if response.status_code == 200:
            return True
    except:
        pass
    return False

def main():
    """Main function"""
    print_header()
    
    # Check API status
    print(f"{Colors.CYAN}Checking API status...{Colors.END}")
    if not check_api_status():
        print(f"\n{Colors.RED}{Colors.BOLD}ERROR:{Colors.END} {Colors.RED}API server is not running!{Colors.END}\n")
        print("Please start the API server first:")
        print(f"  {Colors.CYAN}cd /home/codename/ussd-insurance-service{Colors.END}")
        print(f"  {Colors.CYAN}dotnet run{Colors.END}\n")
        print("Then run this simulator again.\n")
        sys.exit(1)
    
    print(f"{Colors.GREEN}✓ API server is running{Colors.END}\n")
    time.sleep(1)
    
    while True:
        choice = show_quick_test_menu()
        
        if choice is None:
            print(f"\n{Colors.YELLOW}Thank you for using the USSD Simulator!{Colors.END}\n")
            break
        elif choice == 'interactive':
            simulate_ussd_session(PHONE_NUMBER)
        else:
            run_automated_scenario(choice, PHONE_NUMBER)

if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        print(f"\n\n{Colors.YELLOW}Simulator interrupted. Goodbye!{Colors.END}\n")
        sys.exit(0)
