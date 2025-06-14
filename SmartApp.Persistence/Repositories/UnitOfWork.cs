using Microsoft.EntityFrameworkCore;
using SmartApp.Application.Interfaces.Repositories;

namespace SmartApp.Persistence.Repositories
{

    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly DbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(DbContext context)
        {
            _context = context;
        }

        public IRepository<T> Repository<T>() where T : class
        {
            if (_repositories.TryGetValue(typeof(T), out var repo))
                return (IRepository<T>)repo;

            var repositoryInstance = new Repository<T>(_context);
            _repositories[typeof(T)] = repositoryInstance;
            return repositoryInstance;
        }

        public async Task<string> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            try
            {
                if (await _context.SaveChangesAsync(cancellationToken)>1)
                    return "success";
                else
                    return "No record saved";

            }
            catch (DbUpdateException ex)
            {
                return "Database update failed: " + ex.Message;
            }
            catch (Exception ex)
            {
                return "An error occurred during SaveChangesAsync:" + ex.Message;
            }
        }


        public void Dispose()
            => _context.Dispose();

    }
}
