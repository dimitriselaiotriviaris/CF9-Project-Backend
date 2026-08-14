using CF9Project.Core;
using CF9Project.Core.Filters;
using CF9Project.DTO;
using CF9Project.Models;

namespace CF9Project.Services
{
    public interface IUserService
    {
        Task<User> VerifyAndGetUserAsync(UserLoginDTO credentials);
        Task<UserReadOnlyDTO?> GetUserByUsernameAsync(string username);
        Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedUsersFilteredAsync(int pageNumber, 
            int pageSize, UserFiltersDTO userFiltersDTO);
        Task<User> RegisterAsync(RegisterDTO request);
    }
}
