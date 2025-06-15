using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartApp.Shared.Common;
using SmartApp.Application.Interfaces.Repositories;

namespace SmartApp.Persistence.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public IQueryable<T> Query(bool asNoTracking = true)
    {
        return asNoTracking ? _dbSet.AsNoTracking() : _dbSet;
    }

    public async Task<Response<T>> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync(new[] { id }, cancellationToken);
        return entity != null
            ? Response<T>.SuccessResponse(entity)
            : Response<T>.Failure("Not found");
    }
        public async Task<Response<T>> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return Response<T>.SuccessResponse(entity, "Successfully created");
        }
        public Task<Response<T>> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.FromResult(Response<T>.SuccessResponse(entity, "successfully updated"));
    }

    public async Task<Response<bool>> DeleteAsync(object id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync(new[] { id }, cancellationToken);
        if (entity == null)
            return Response<bool>.Failure("Not found to delete");

        _dbSet.Remove(entity);
        return Response<bool>.SuccessResponse(true, "successfully deleted");
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate is not null)
            return await _dbSet.CountAsync(predicate, cancellationToken);

        return await _dbSet.CountAsync(cancellationToken);
    }

    public async Task<Response<PagedResult<T>>> GetPagedAsync(
        Expression<Func<T, bool>>? filter,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var result = new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        return Response<PagedResult<T>>.SuccessResponse(result);
    }
    public async Task<Response<T>> SqlQuerySingleAsync(string sql,object[]? parameters = null,CancellationToken cancellationToken = default)
        {
            var result = await _dbSet
                .FromSqlRaw(sql, parameters ?? Array.Empty<object>())
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (result == null)
                return Response<T>.Failure("No record found");

            return Response<T>.SuccessResponse(result);
        }

        public async Task<Response<IEnumerable<T>>> SqlQueryListAsync(
        string sql,
        object[]? parameters = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _dbSet
            .FromSqlRaw(sql, parameters ?? Array.Empty<object>())
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Response<IEnumerable<T>>.SuccessResponse(result);
    }

    public async Task<Response<int>> ExecuteSqlCommandAsync(
        string sql,
        object[]? parameters = null,
        CancellationToken cancellationToken = default)
    {
        var rows = await _context.Database.ExecuteSqlRawAsync(sql, parameters ?? Array.Empty<object>(), cancellationToken);
        return Response<int>.SuccessResponse(rows, "Command executed.");
    }


    }


}
