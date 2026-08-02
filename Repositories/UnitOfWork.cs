
using Microsoft.EntityFrameworkCore;
using CF9Project.Data;
using CF9Project.Models;
using System.Diagnostics;

namespace CF9Project.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Data.CF9ProjectContext _context;
        public IUserRepository UserRepository { get; }
        public ICompanyRepository CompanyRepository { get; }
        public IGamerRepository GamerRepository { get; }
        public IGameRepository GameRepository { get; }

        public UnitOfWork(Data.CF9ProjectContext context)
        {
            _context = context;
            UserRepository = new UserRepository(context);
            GamerRepository = new GamerRepository(context);
            CompanyRepository = new CompanyRepository(context);
            GameRepository = new GameRepository(context);
        }

        public async Task<bool> SaveAsync()
        {
            foreach (var entry in _context.ChangeTracker.Entries<User>())
            {
                if (entry.State == EntityState.Added)
                {
                    Debug.WriteLine($"NEW USER ID: {entry.Entity.Id}");
                    Debug.WriteLine($"NEW USER ROLE ID: {entry.Entity.RoleId}");
                    Debug.WriteLine($"NEW USER ROLE: {entry.Entity.Role?.Name ?? "NULL"}");
                }
            }
            try
            {
                return await _context.SaveChangesAsync() > 0;    // κάνει commit και αυτόματα rollback αν αποτύχει
            }
            catch (DbUpdateException ex)
            {
                Debug.WriteLine("========== EF SAVE ERROR ==========");
                Debug.WriteLine(ex.ToString());

                Debug.WriteLine("========== INNER EXCEPTION ==========");
                Debug.WriteLine(ex.InnerException?.ToString());

                throw;
            }

        }
    }
}
