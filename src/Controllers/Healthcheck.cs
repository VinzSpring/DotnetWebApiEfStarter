using System.Security.Claims;
using backend.Entities;
using backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class HealthCheckController : ControllerBase
{
    public HealthCheckController()
    {
    }

    [HttpGet]
    public IActionResult Get()
    {
        var currentAssemblyVersion = typeof(HealthCheckController).Assembly.GetName().Version;
        return Ok(new
        {
            Version = currentAssemblyVersion?.ToString(),
            Status = "Healthy"
        });
    }
}
