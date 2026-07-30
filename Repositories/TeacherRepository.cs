using Microsoft.EntityFrameworkCore;
using SchoolApp.Core;
using SchoolApp.Data;
using SchoolApp.Models;
using System.Linq.Expressions;

namespace SchoolApp.Repositories
{
    public class TeacherRepository : BaseRepository<Teacher>, ITeacherRepository
    {
        public TeacherRepository(SchoolMvc9Context context) : base(context)
        {
        }

        public async Task<List<Course>> GetTeacherCoursesAsync(int teacherId)
        {
            List<Course> courses;

            courses = await _context.Teachers
                .Where(t => t.Id == teacherId)
                .SelectMany(t => t.Courses)
                .ToListAsync();

            return courses;

        }

        public async Task<Teacher?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Teachers
                .Where(t => t.PhoneNumber == phoneNumber)
                .FirstOrDefaultAsync(); // fetch and select the first or default
        }


        public async Task<User?> GetUserTeacherByUsernameAsync(string username)
        {
            var userTeacher = await _context.Users
                .Include(u => u.Teacher) // Εager loading της σχετικής οντότητας Teacher
                .Where(u => u.Username == username && u.Teacher != null)
                .SingleOrDefaultAsync();    // fetces 0 or 1 results, otherwise throws an exception

            return userTeacher;
        }

        public async Task<PaginatedResult<User>> GetPaginatedUsersTeachersAsync(int pageNumber, int pageSize)
        {
            int skip = (pageNumber - 1) * pageSize;

            var usersWithRoleTeacher = await _context.Users
                .Include(u => u.Teacher) // Εager loading της σχετικής οντότητας Teacher  
                .Where(u => u.Teacher != null)           
                .OrderBy(u => u.Id) // πάντα να υπάρχει ένα OrderBy πριν το Skip
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            int totalRecords = await _context.Users
                .Where(u => u.Teacher != null)
                .CountAsync();

            return new PaginatedResult<User>(usersWithRoleTeacher, totalRecords, pageNumber, pageSize);
        }     

        public async Task<PaginatedResult<User>> GetPaginatedUsersTeachersFilteredAsync(int pageNumber, int pageSize, 
            List<Expression<Func<User, bool>>> predicates)
        {
            IQueryable<User> query = _context.Users
                .Include(u => u.Teacher) // Εager loading της σχετικής οντότητας Teacher
                .Where(u => u.Teacher != null);
                

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
