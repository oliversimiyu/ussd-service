namespace UssdInsuranceService.Models;

public class UssdRequest
{
    public string SessionId { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string ServiceCode { get; set; } = string.Empty;
}

public class UssdResponse
{
    public string Message { get; set; } = string.Empty;
    public bool EndSession { get; set; }

    public override string ToString()
    {
        return EndSession ? $"END {Message}" : $"CON {Message}";
    }
}

public class UssdSession
{
    public string SessionId { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string CurrentMenu { get; set; } = "main";
    public Dictionary<string, string> Data { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow;
}
