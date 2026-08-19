using System.Security.Claims;
using CF9Project.Data;
using CF9Project.DTO;
using CF9Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CF9Project.Controllers.Api;

[ApiController]
[Route("api/company")]
[Authorize(Roles = "COMPANY")]
public class CompanyController : ControllerBase
{
    private readonly CF9ProjectContext _context;

    public CompanyController(CF9ProjectContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(value, out var userId))
        {
            throw new UnauthorizedAccessException();
        }

        return userId;
    }

    private async Task<Company> GetOrCreateCompanyAsync()
    {
        var userId = GetCurrentUserId();

        var company = await _context.Companies
            .FirstOrDefaultAsync(c =>
                c.UserId == userId &&
                !c.IsDeleted);

        if (company != null)
        {
            return company;
        }

        company = new Company
        {
            UserId = userId,
            InsertedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        return company;
    }

    [HttpGet("games")]
    public async Task<ActionResult<List<GameReadOnlyDTO>>> GetGames()
    {
        var company = await GetOrCreateCompanyAsync();

        var games = await _context.Games
            .Where(g =>
                g.CompanyId == company.Id &&
                !g.IsDeleted)
            .OrderBy(g => g.Name)
            .Select(g => new GameReadOnlyDTO
            {
                Id = g.Id,
                Name = g.Name,
                Price = g.Price,
                Description = g.Description
            })
            .ToListAsync();

        return Ok(games);
    }

    [HttpPost("games")]
    public async Task<ActionResult<GameReadOnlyDTO>> CreateGame(
        [FromBody] GameCreateDTO dto)
    {
        var company = await GetOrCreateCompanyAsync();

        var game = new Game
        {
            Name = dto.Name,
            Price = dto.Price,
            Description = dto.Description ?? string.Empty,

            CompanyId = company.Id,

            InsertedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetGames),
            new
            {
                id = game.Id
            },
            new GameReadOnlyDTO
            {
                Id = game.Id,
                Name = game.Name,
                Price = game.Price,
                Description = game.Description
            });
    }

    [HttpPut("games/{id:int}")]
    public async Task<ActionResult<GameReadOnlyDTO>> UpdateGame(
        int id,
        [FromBody] GameUpdateDTO dto)
    {
        var company = await GetOrCreateCompanyAsync();

        var game = await _context.Games
            .FirstOrDefaultAsync(g =>
                g.Id == id &&
                g.CompanyId == company.Id &&
                !g.IsDeleted);

        if (game == null)
        {
            return NotFound();
        }

        game.Name = dto.Name;
        game.Price = dto.Price;
        game.Description = dto.Description ?? string.Empty;
        game.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new GameReadOnlyDTO
        {
            Id = game.Id,
            Name = game.Name,
            Price = game.Price,
            Description = game.Description
        });
    }
}