using Microsoft.EntityFrameworkCore;
using SmartApp.Application.Interfaces.Repositories;
using SmartApp.Persistence.DBContext;
using SmartApp.Shared.Common;

namespace SmartApp.Persistence.Repositories
{

    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(ApplicationDbContext context)
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

        public async Task<Response<int>> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var count = await _context.SaveChangesAsync(cancellationToken);
                if (count > 0)
                    return Response<int>.SuccessResponse(count, "Save successful");
                else
                    return Response<int>.Failure("No records were saved");
            }
            catch (DbUpdateException ex)
            {
                return Response<int>.Failure("Database update failed: " + ex.Message);
            }
            catch (Exception ex)
            {
                return Response<int>.Failure("An error occurred during SaveChangesAsync: " + ex.Message);
            }
        }



        public void Dispose()
            => _context.Dispose();

    }
}
