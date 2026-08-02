
using CF9Project.Models;

namespace CF9Project.Repositories
{
    public interface IGameRepository : IBaseRepository<Game>
    {
        Task<List<Gamer>> GetGameGamersAsync(int courseId);
        Task<Company?> GetGameCompanyAsync(int courseId);
    }
}
