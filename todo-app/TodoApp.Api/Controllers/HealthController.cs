using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TodoApp.Api.Data;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[DisableRateLimiting]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<HealthController> _logger;
    private const int TimeoutSeconds = 5;

    public HealthController(AppDbContext context, ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Check()
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(TimeoutSeconds));

            var canConnect = await _context.Database.CanConnectAsync(cts.Token);

            if (canConnect)
            {
                return Ok(new { status = "ok", database = "connected" });
            }

            _logger.LogWarning("Health check failed: database not connected");
            return StatusCode(503, new { status = "error", database = "disconnected" });
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Health check failed: database connection timeout");
            return StatusCode(503, new { status = "error", database = "disconnected" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed with exception");
            return StatusCode(503, new { status = "error", database = "disconnected" });
        }
    }
}
