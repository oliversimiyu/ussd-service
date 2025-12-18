using System.Reflection;
using UssdInsuranceService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "USSD Insurance Service API",
        Version = "v1",
        Description = "A comprehensive USSD-based insurance management system that enables users to manage policies, submit claims, and make payments through a simple USSD interface.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Insurance Support",
            Email = "support@insurance.com",
            Url = new Uri("https://insurance.com/support")
        }
    });

    // Enable XML documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Register services
builder.Services.AddSingleton<ISessionManager, SessionManager>();
builder.Services.AddScoped<IUssdMenuService, UssdMenuService>();
builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "USSD Insurance Service API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "USSD Insurance Service - API Documentation";
    });
}

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    timestamp = DateTime.UtcNow,
    service = "USSD Insurance Service"
})).WithName("HealthCheck").WithTags("Health");

// API info endpoint
app.MapGet("/", () => Results.Ok(new
{
    service = "USSD Insurance Service",
    version = "1.0.0",
    documentation = "/swagger",
    health = "/health"
})).WithName("ApiInfo").WithTags("Info");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
