using CF9Project.Core;
using CF9Project.Data;
using CF9Project.DTO;
using CF9Project.Models;

namespace CF9Project.Services
{
    public interface IGamerService
    {
        Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedGamersAsync(int pageNumber, int pageSize);
    }
}