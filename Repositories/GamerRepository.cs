using Microsoft.EntityFrameworkCore;
using CF9Project.Core;
using CF9Project.Data;
using CF9Project.Models;
using System.Linq.Expressions;

namespace CF9Project.Repositories
{
    public class GamerRepository : BaseRepository<Gamer>, IGamerRepository
    {
        public GamerRepository(Data.CF9ProjectContext context) : base(context)
        {
        }

        public async Task<PaginatedResult<User>> GetPaginatedUsersGamersAsync(int pageNumber, int pageSize)
        {
            int skip = (pageNumber - 1) * pageSize;

            var usersWithRoleStudent = await _context.Users
                .Include(u => u.Gamer) // Εager loading της σχετικής οντότητας Student   
                .Where(u => u.Gamer != null)   
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            int totalRecords = await _context.Users
                .Where(u => u.Gamer != null)
                .CountAsync();

            return new PaginatedResult<User>(usersWithRoleStudent, totalRecords, pageNumber, pageSize);
        }

        public async Task<PaginatedResult<Gamer>> GetPaginatedUsersGamersFilteredAsync(int pageNumber, 
            int pageSize, List<Expression<Func<Gamer, bool>>> predicates)
        {
            IQueryable<Gamer> query = _context.Gamers;

            // Apply predicates as Expression<Func<Student, bool>> so they run in DB
            if (predicates != null && predicates.Count > 0)
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate);
                }
            }

            // Get total count BEFORE pagination
            int totalRecords = await query.CountAsync();

            // Paginate AFTER filtering
            int skip = (pageNumber - 1) * pageSize;

            var data = await query
                .OrderBy(u => u.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Gamer>
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<List<Game>> GetGamerGamesAsync(int gamerId)
        {
            List<Game> courses;

            courses = await _context.Gamers
                .Where(s => s.Id == gamerId)
                .SelectMany(c => c.Games)
                .ToListAsync();

            return courses;
        }
    }
}
