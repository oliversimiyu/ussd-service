# Contributing to USSD Insurance Service

Thank you for your interest in contributing to the USSD Insurance Service! This document provides guidelines and instructions for contributing to this project.

## Table of Contents
- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Process](#development-process)
- [Coding Standards](#coding-standards)
- [Submitting Changes](#submitting-changes)
- [Reporting Bugs](#reporting-bugs)
- [Feature Requests](#feature-requests)

## Code of Conduct

This project adheres to a code of conduct that all contributors are expected to follow. Please be respectful and constructive in all interactions.

## Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- Git
- A code editor (Visual Studio 2022, VS Code, or JetBrains Rider)
- Docker (optional, for containerized testing)

### Setting Up Your Development Environment

1. **Fork the repository**
   ```bash
   # Fork on GitHub, then clone your fork
   git clone https://github.com/YOUR_USERNAME/ussd-insurance-service.git
   cd ussd-insurance-service
   ```

2. **Add upstream remote**
   ```bash
   git remote add upstream https://github.com/ORIGINAL_OWNER/ussd-insurance-service.git
   ```

3. **Create a branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

4. **Install dependencies**
   ```bash
   dotnet restore
   ```

5. **Build the project**
   ```bash
   dotnet build
   ```

6. **Run the project**
   ```bash
   dotnet run
   ```

## Development Process

### Branch Naming Convention
- `feature/` - New features (e.g., `feature/add-sms-notifications`)
- `bugfix/` - Bug fixes (e.g., `bugfix/fix-payment-calculation`)
- `hotfix/` - Urgent production fixes
- `docs/` - Documentation updates
- `refactor/` - Code refactoring
- `test/` - Adding or updating tests

### Commit Message Guidelines

Follow the conventional commits format:

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `chore`: Maintenance tasks

**Examples:**
```
feat(payment): add M-Pesa integration

Implemented M-Pesa STK push for premium payments.
Added payment callback handling and status updates.

Closes #123
```

```
fix(claims): correct claim amount calculation

Fixed decimal precision issue in claim amount calculation
that was causing rounding errors.

Fixes #456
```

## Coding Standards

### C# Style Guidelines

1. **Naming Conventions**
   - PascalCase for classes, methods, properties: `PolicyService`, `CreatePolicy`
   - camelCase for local variables and parameters: `policyId`, `customerName`
   - Private fields with underscore prefix: `_policyService`

2. **Code Organization**
   - Keep methods focused and small (< 30 lines ideally)
   - One class per file
   - Use meaningful names
   - Add XML documentation comments for public APIs

3. **Formatting**
   - Use 4 spaces for indentation (no tabs)
   - Place opening braces on new lines
   - Maximum line length: 120 characters

4. **SOLID Principles**
   - Follow dependency injection patterns
   - Use interfaces for service contracts
   - Keep classes focused on single responsibility

### Example Code Style

```csharp
/// <summary>
/// Processes a premium payment for the specified policy
/// </summary>
/// <param name="policyId">The policy identifier</param>
/// <returns>Payment confirmation details</returns>
public async Task<Payment> ProcessPayment(int policyId)
{
    var policy = await _policyService.GetPolicyById(policyId);
    
    if (policy == null)
    {
        throw new NotFoundException($"Policy {policyId} not found");
    }

    var payment = new Payment
    {
        PolicyId = policyId,
        Amount = policy.Premium,
        Status = PaymentStatus.Pending
    };

    return await _paymentService.ProcessAsync(payment);
}
```

## Submitting Changes

### Pull Request Process

1. **Update your fork**
   ```bash
   git fetch upstream
   git rebase upstream/main
   ```

2. **Ensure all tests pass**
   ```bash
   dotnet test
   ```

3. **Ensure code builds without warnings**
   ```bash
   dotnet build --no-incremental
   ```

4. **Push your changes**
   ```bash
   git push origin feature/your-feature-name
   ```

5. **Create a Pull Request**
   - Go to your fork on GitHub
   - Click "New Pull Request"
   - Provide a clear title and description
   - Reference any related issues

### Pull Request Checklist

- [ ] Code follows the project's coding standards
- [ ] All tests pass
- [ ] New features have appropriate tests
- [ ] Documentation has been updated
- [ ] Commit messages follow guidelines
- [ ] No merge conflicts
- [ ] Code has been reviewed by yourself first

### Pull Request Description Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Related Issues
Closes #(issue number)

## Testing
Describe how you tested these changes

## Checklist
- [ ] Code follows style guidelines
- [ ] Self-review completed
- [ ] Comments added for complex code
- [ ] Documentation updated
- [ ] No new warnings generated
- [ ] Tests added/updated
- [ ] All tests passing
```

## Reporting Bugs

### Before Submitting a Bug Report

1. Check existing issues to avoid duplicates
2. Verify the bug with the latest version
3. Collect relevant information:
   - OS and version
   - .NET version
   - Steps to reproduce
   - Expected vs actual behavior
   - Error messages/stack traces
   - Screenshots (if applicable)

### Bug Report Template

```markdown
## Bug Description
Clear and concise description of the bug

## Steps to Reproduce
1. Go to '...'
2. Click on '...'
3. Scroll down to '...'
4. See error

## Expected Behavior
What you expected to happen

## Actual Behavior
What actually happened

## Environment
- OS: [e.g., Windows 11, Ubuntu 22.04]
- .NET Version: [e.g., 8.0.1]
- Browser (if applicable): [e.g., Chrome 120]

## Additional Context
Any other relevant information

## Screenshots
If applicable, add screenshots
```

## Feature Requests

We welcome feature requests! Please provide:

1. **Use Case**: Describe the problem you're trying to solve
2. **Proposed Solution**: Your idea for how to solve it
3. **Alternatives**: Other solutions you've considered
4. **Additional Context**: Any other relevant information

### Feature Request Template

```markdown
## Problem Statement
Clear description of the problem or need

## Proposed Solution
Detailed description of your proposed solution

## Alternatives Considered
Other approaches you've thought about

## Benefits
How this would help users

## Implementation Complexity
Your assessment: Low / Medium / High

## Additional Context
Any other relevant details
```

## Development Guidelines

### Adding New Features

1. **Check existing functionality** - Ensure the feature doesn't already exist
2. **Discuss major changes** - Open an issue first for significant features
3. **Write tests** - Include unit tests for new functionality
4. **Update documentation** - Keep README and API docs current
5. **Follow patterns** - Match existing code structure and patterns

### Testing

- Write unit tests for business logic
- Add integration tests for API endpoints
- Ensure test coverage for new code
- Use meaningful test names that describe what's being tested

```csharp
[Fact]
public async Task CreatePolicy_WithValidData_ReturnsPolicy()
{
    // Arrange
    var service = new PolicyService();
    var policy = new Policy { /* ... */ };

    // Act
    var result = await service.CreatePolicy(policy);

    // Assert
    Assert.NotNull(result);
    Assert.NotNull(result.PolicyNumber);
}
```

### Database Changes

If adding database changes:
1. Create appropriate migration scripts
2. Update seed data if necessary
3. Document schema changes
4. Consider backward compatibility

### API Changes

For API modifications:
1. Follow RESTful principles
2. Maintain backward compatibility when possible
3. Version breaking changes
4. Update Swagger documentation
5. Update example requests/responses

## Questions?

If you have questions about contributing:
- Open a discussion on GitHub
- Check existing issues and pull requests
- Review closed issues for similar questions

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

## Thank You!

Your contributions help make this project better for everyone. We appreciate your time and effort! 🙏
