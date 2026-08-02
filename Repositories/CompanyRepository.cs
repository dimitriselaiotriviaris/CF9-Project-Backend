using Microsoft.EntityFrameworkCore;
using CF9Project.Core;
using CF9Project.Data;
using CF9Project.Models;
using System.Linq.Expressions;

namespace CF9Project.Repositories
{
    public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(Data.CF9ProjectContext context) : base(context)
        {
        }

        public async Task<List<Game>> GetCompanyGamesAsync(int companyId)
        {
            List<Game> games;

            games = await _context.Companies
                .Where(c => c.Id == companyId)
                .SelectMany(c => c.Games)
                .ToListAsync();

            return games;

        }

        public async Task<User?> GetUserCompanyByUsernameAsync(string username)
        {
            var userCompany = await _context.Users
                .Include(u => u.Company) // Εager loading της σχετικής οντότητας Company
                .Where(u => u.Username == username && u.Company != null)
                .SingleOrDefaultAsync();    // fetces 0 or 1 results, otherwise throws an exception

            return userCompany;
        }

        public async Task<PaginatedResult<User>> GetPaginatedUsersCompanyAsync(int pageNumber, int pageSize)
        {
            int skip = (pageNumber - 1) * pageSize;

            var usersWithRoleCompany = await _context.Users
                .Include(u => u.Company) // Εager loading της σχετικής οντότητας Company
                .Where(u => u.Company != null)
                .OrderBy(u => u.Id) // πάντα να υπάρχει ένα OrderBy πριν το Skip
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            int totalRecords = await _context.Users
                .Where(u => u.Company != null)
                .CountAsync();

            return new PaginatedResult<User>(usersWithRoleCompany, totalRecords, pageNumber, pageSize);
        }     

        public async Task<PaginatedResult<User>> GetPaginatedUsersCompanyFilteredAsync(int pageNumber, int pageSize, 
            List<Expression<Func<User, bool>>> predicates)
        {
            IQueryable<User> query = _context.Users
                .Include(u => u.Company) // Εager loading της σχετικής οντότητας Company
                .Where(u => u.Company != null);
                

            if (predicates != null && predicates.Count > 0) 
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate); // εκτελείται, υπονοείται το AND
                }
            }

            int totalRecords = await query.CountAsync(); // εκτελείται
            int skip = (pageNumber - 1) * pageSize;
            
            var data = await query
                .OrderBy(u => u.Id) // πάντα να υπάρχει ένα OrderBy πριν το Skip
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(); // εκτελείται

            return new PaginatedResult<User>(data, totalRecords, pageNumber, pageSize);
        }
    }
}
