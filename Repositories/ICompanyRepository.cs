using CF9Project.Core;
using CF9Project.Data;
using CF9Project.Models;
using System.Linq.Expressions;

namespace CF9Project.Repositories
{
    public interface ICompanyRepository : IBaseRepository<Company>
    {
        Task<List<Game>> GetCompanyGamesAsync(int companyId);
        Task<User?> GetUserCompanyByUsernameAsync(string username);
        
        Task<PaginatedResult<User>> GetPaginatedUsersCompanyAsync(int pageNumber, int pageSize);
        Task<PaginatedResult<User>> GetPaginatedUsersCompanyFilteredAsync(int pageNumber, int pageSize, 
            List<Expression<Func<User, bool>>> predicates);
    }
}
