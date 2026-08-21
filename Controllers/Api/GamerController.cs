using System.Security.Claims;
using CF9Project.Data;
using CF9Project.DTO;
using CF9Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CF9Project.Controllers.Api;

[ApiController]
[Route("api/gamer")]
[Authorize(Roles = "GAMER")]
public class GamerController : ControllerBase
{
    private readonly CF9ProjectContext _context;

    public GamerController(CF9ProjectContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var value =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(value, out var userId))
        {
            throw new UnauthorizedAccessException();
        }

        return userId;
    }

    private async Task<Gamer> GetOrCreateGamerAsync()
    {
        var userId = GetCurrentUserId();

        var gamer = await _context.Gamers
            .FirstOrDefaultAsync(g =>
                g.UserId == userId &&
                !g.IsDeleted);

        if (gamer != null)
        {
            return gamer;
        }

        gamer = new Gamer
        {
            UserId = userId,
            InsertedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        _context.Gamers.Add(gamer);
        await _context.SaveChangesAsync();

        return gamer;
    }

    // ALL games from ALL companies
    [HttpGet("games")]
    public async Task<ActionResult<List<GamerGameReadOnlyDTO>>>
        GetAllGames()
    {
        var games = await _context.Games
            .Where(g => !g.IsDeleted)
            .OrderBy(g => g.Name)
            .Select(g => new GamerGameReadOnlyDTO
            {
                Id = g.Id,
                Name = g.Name,
                Price = g.Price,
                Description = g.Description,
                CompanyUsername = g.Company!.User.Username
            })
            .ToListAsync();

        return Ok(games);
    }

    // Only this gamer's saved games
    [HttpGet("library")]
    public async Task<ActionResult<List<GamerGameReadOnlyDTO>>>
        GetLibrary()
    {
        var gamer =
            await GetOrCreateGamerAsync();

        var games = await _context.Games
            .Where(game =>
                game.Gamers.Any(g =>
                    g.Id == gamer.Id) &&
                !game.IsDeleted)
            .OrderBy(game => game.Name)
            .Select(game => new GamerGameReadOnlyDTO
            {
                Id = game.Id,
                Name = game.Name,
                Price = game.Price,
                Description = game.Description,
                CompanyUsername = game.Company!.User.Username
            })
            .ToListAsync();

        return Ok(games);
    }

    [HttpPost("library/{gameId:int}")]
    public async Task<IActionResult> AddToLibrary(
        int gameId)
    {
        var gamer =
            await GetOrCreateGamerAsync();

        var game = await _context.Games
            .FirstOrDefaultAsync(g =>
                g.Id == gameId &&
                !g.IsDeleted);

        if (game == null)
        {
            return NotFound(new
            {
                message = "Game not found."
            });
        }

        var alreadyAdded = await _context.Gamers
            .Where(g => g.Id == gamer.Id)
            .AnyAsync(g =>
                g.Games.Any(game =>
                    game.Id == gameId));

        if (alreadyAdded)
        {
            return Conflict(new
            {
                message =
                    "Game is already in your library."
            });
        }

        gamer.Games.Add(game);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("library/{gameId:int}")]
    public async Task<IActionResult> RemoveFromLibrary(
        int gameId)
    {
        var gamer =
            await GetOrCreateGamerAsync();

        var game = await _context.Games
            .FirstOrDefaultAsync(g =>
                g.Id == gameId);

        if (game == null)
        {
            return NotFound();
        }

        await _context.Entry(gamer)
            .Collection(g => g.Games)
            .LoadAsync();

        if (!gamer.Games.Contains(game))
        {
            return NotFound(new
            {
                message =
                    "Game is not in your library."
            });
        }

        gamer.Games.Remove(game);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}