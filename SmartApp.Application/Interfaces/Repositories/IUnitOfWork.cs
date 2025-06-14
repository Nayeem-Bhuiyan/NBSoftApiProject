using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApp.Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
