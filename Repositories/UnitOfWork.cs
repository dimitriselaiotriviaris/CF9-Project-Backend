
using Microsoft.EntityFrameworkCore;
using SchoolApp.Data;
using SchoolApp.Models;
using System.Diagnostics;

namespace SchoolApp.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SchoolMvc9Context _context;
        public IUserRepository UserRepository { get; }
        public ITeacherRepository TeacherRepository { get; }
        public IStudentRepository StudentRepository { get; }
        public ICourseRepository CourseRepository { get; }

        public UnitOfWork(SchoolMvc9Context context)
        {
            _context = context;
            UserRepository = new UserRepository(context);
            StudentRepository = new StudentRepository(context);
            TeacherRepository = new TeacherRepository(context);
            CourseRepository = new CourseRepository(context);
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
