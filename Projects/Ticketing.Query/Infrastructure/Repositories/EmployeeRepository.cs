using Microsoft.EntityFrameworkCore;
using Ticketing.Query.Domain.Employees;
using Ticketing.Query.Infrastructure.Persistence;

namespace Ticketing.Query.Infrastructure.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(TicketDbContext context) : base(context)
        {
        }
        public async Task<Employee?> GetByUsernameAsync(string username)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e != null && e.Email == username);
        }
    }
}
