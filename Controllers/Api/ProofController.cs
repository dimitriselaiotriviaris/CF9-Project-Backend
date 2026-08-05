using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CF9Project.Data;

namespace CF9Project.Controllers.Api;

[ApiController]
[Route("api/proof")]
public class ProofController : ControllerBase
{
    private readonly CF9ProjectContext _context;

    public ProofController(CF9ProjectContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Get()
    {
        return Ok(new
        {
            message = "SchoolApp API is reachable",
            serverTimeUtc = DateTime.UtcNow,
            authenticated = User.Identity?.IsAuthenticated ?? false
        });
    }

    [HttpGet("database")]
    [AllowAnonymous]
    public async Task<IActionResult> Database(CancellationToken cancellationToken)
    {
        var canConnect = await _context.Database.CanConnectAsync(cancellationToken);

        if (!canConnect)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                connected = false,
                message = "The API is running, but SQL Server is unavailable."
            });
        }

        var roleCount = await _context.Roles.CountAsync(cancellationToken);

        return Ok(new
        {
            connected = true,
            database = _context.Database.GetDbConnection().Database,
            roleCount
        });
    }
}
