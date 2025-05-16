using System.Collections;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Domain.Employees;
using Ticketing.Query.Infrastructure.Repositories;

namespace Ticketing.Query.Infrastructure.Persistence
{
    //Administrator of repositories 
    public class UnitOfWork: IUnitOfWork
    {
        private Hashtable _repositories = new();
        private DataBaseContextFactory _contextFactory;
        private readonly TicketDbContext _context;
        private IEmployeeRepository? _employeeRepository;

        public IEmployeeRepository EmployeeRepository => _employeeRepository ??= new EmployeeRepository(_context);
        public UnitOfWork(DataBaseContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
            _context = _contextFactory.CreateDbContext();
        }

        public IGenericRepository<TEntity> RepositoryGeneric<TEntity>() where TEntity : class
        {
            if (_repositories is not null)
            {
                _repositories = new Hashtable();
            }
            var type = typeof(TEntity).Name;
            if (!_repositories!.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>);
                var repositoryInstance =
                    Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);
                _repositories.Add(type, repositoryInstance);
            }
            return (_repositories[type] as IGenericRepository<TEntity>)!;
        }

        public async Task<int> Complete()
        {
           return await _context.SaveChangesAsync();
        }
    }
}
