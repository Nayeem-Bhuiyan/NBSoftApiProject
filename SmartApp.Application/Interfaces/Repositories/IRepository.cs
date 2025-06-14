using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using SmartApp.Shared.Common;
namespace SmartApp.Application.Interfaces.Repositories
{
    

    public interface IRepository<T> where T : class
    {
        IQueryable<T> Query();

        Task<Response<T>> GetByIdAsync(object id, CancellationToken cancellationToken = default);
        Task<Response<T>> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<Response<T>> UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task<Response<bool>> DeleteAsync(object id, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
        Task<Response<PagedResult<T>>> GetPagedAsync(
            Expression<Func<T, bool>>? filter,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default
        );

        Task<Response<IEnumerable<T>>> ExecuteSqlAsync(
            string sql,
            object[]? parameters = null,
            CancellationToken cancellationToken = default
        );

        Task<Response<int>> ExecuteSqlCommandAsync(
            string sql,
            object[]? parameters = null,
            CancellationToken cancellationToken = default
        );
    }

}
