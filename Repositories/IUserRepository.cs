using CF9Project.Core;
using CF9Project.Models;
using System.Linq.Expressions;

namespace CF9Project.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        //Task<User?> GetUserAsync(string username, string password);
        //Task<User?> GetUserAsync(string username);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<PaginatedResult<User>> GetUsersAsync(int pageNumber, int pageSize,
           List<Expression<Func<User, bool>>> predicates);
    }
}
