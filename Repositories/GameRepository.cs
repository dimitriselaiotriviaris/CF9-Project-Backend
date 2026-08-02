using Microsoft.EntityFrameworkCore;
using CF9Project.Data;
using CF9Project.Models;

namespace CF9Project.Repositories
{
    public class GameRepository : BaseRepository<Game>, IGameRepository
    {
        public GameRepository(Data.CF9ProjectContext context) : base(context)
        {
        }

        public async Task<List<Gamer>> GetGameGamersAsync(int courseId)
        {
            return await _context.Games
               .Where(c => c.Id == courseId)
               .SelectMany(c => c.Gamers)
               .ToListAsync();
        }

        public async Task<Company?> GetGameCompanyAsync(int courseId)
        {
           
            var course = await _context.Games
                    .Include(c => c.Company) // eagerly loads related entities in the same query
                    .FirstOrDefaultAsync(c => c.Id == courseId);

            return course?.Company; // not second query, since teacher has loaded
        }
    }
}
