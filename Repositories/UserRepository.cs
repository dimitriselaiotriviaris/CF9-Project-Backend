using Microsoft.EntityFrameworkCore;
using SchoolApp.Core;
using SchoolApp.Data;
using SchoolApp.Models;
using SchoolApp.Security;
using System.Linq.Expressions;

namespace SchoolApp.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
       
        public UserRepository(SchoolMvc9Context context) : base(context)
        {
            
        }

        //public async Task<User?> GetUserAsync(string username, string password)
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username 
        //    || u.Email == username);

        //    if (user == null) return null;

        //    if (!_encryptionUtil.IsValidPassword(password, user.Password)) return null;

        //    return user;
        //}

        //public async Task<User?> GetUserAsync(string username)
        //{
        //    return await _context.Users
        //        .FirstOrDefaultAsync(u => u.Username == username
        //        || u.Email == username);
        //}

        public async Task<User?> GetUserByUsernameAsync(string username) =>
            await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == username || u.Email == username);
        

        public async Task<PaginatedResult<User>> GetUsersAsync(int pageNumber, int pageSize, 
            List<Expression<Func<User, bool>>> predicates)
        {
            IQueryable<User> query = _context.Users; // δεν εκτελείται

            if (predicates != null && predicates.Count > 0)
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate); // υπονοείται το AND
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
