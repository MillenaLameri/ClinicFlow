using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var response = new
        {
            status = "Healthy",
            application = "ClinicFlow API",
            environment = Environment.GetEnvironmentVariable(
                "ASPNETCORE_ENVIRONMENT"
            ),
            timestamp = DateTime.UtcNow
        };

        return Ok(response);
    }
}