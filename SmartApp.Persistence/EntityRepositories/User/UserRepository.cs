using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartApp.Application.DTOs.Auth;
using SmartApp.Application.Interfaces.Repositories;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Persistence.DBContext;
using SmartApp.Shared.Common;

namespace SmartApp.Persistence.EntityRepositories.User;

public sealed class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db          = db;
        _userManager = userManager;
    }

    public async Task<Response<PagedResult<UserSummaryDto>>> GetPagedAsync(
        string? searchTerm,
        string? roleFilter,
        bool? isActive,
        int pageIndex,
        int pageSize,
        string? sortBy,
        bool sortDesc,
        CancellationToken ct = default)
    {
        if (pageIndex < 1) pageIndex = 1;
        if (pageSize  < 1) pageSize  = 10;

        // ← base query with role join
        var query = from user in _db.Users.AsNoTracking()
                    select new
                    {
                        user.Id,
                        user.UserName,
                        user.Email,
                        user.FirstName,
                        user.LastName,
                        user.isActive,
                        Roles = (from ur in _db.UserRoles
                                 join r in _db.Roles on ur.RoleId equals r.Id
                                 where ur.UserId == user.Id
                                 select r.Name).ToList()
                    };

        // ── Filters ───────────────────────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(u =>
                u.UserName.Contains(searchTerm) ||
                u.Email.Contains(searchTerm)    ||
                u.FirstName.Contains(searchTerm)||
                u.LastName.Contains(searchTerm));

        if (!string.IsNullOrWhiteSpace(roleFilter))
            query = query.Where(u => u.Roles.Contains(roleFilter));

        if (isActive.HasValue)
            query = query.Where(u => u.isActive == isActive.Value);

        // ── Sorting ───────────────────────────────────────────────────────
        query = sortBy?.ToLower() switch
        {
            "username" => sortDesc ? query.OrderByDescending(u => u.UserName) : query.OrderBy(u => u.UserName),
            "email" => sortDesc ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "firstname" => sortDesc ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName),
            "lastname" => sortDesc ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName),
            _ => query.OrderBy(u => u.UserName)
        };

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var dtoItems = items.Select(u => new UserSummaryDto
        {
            Id        = u.Id,
            UserName  = u.UserName,
            Email     = u.Email,
            FirstName = u.FirstName,
            LastName  = u.LastName,
            IsActive  = u.isActive,
            Roles     = u.Roles
        }).ToList();

        return Response<PagedResult<UserSummaryDto>>.SuccessResponse(new PagedResult<UserSummaryDto>
        {
            items      = dtoItems,
            totalCount = totalCount,
            pageIndex  = pageIndex,
            pageSize   = pageSize
        }, "Users loaded successfully.");
    }

    public async Task<UserDetailDto> GetDetailByIdAsync(string userId, CancellationToken ct = default)
    {
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null) return null;

        var roles = await _userManager.GetRolesAsync(user);

        return new UserDetailDto
        {
            Id                = user.Id,
            UserName          = user.UserName!,
            Email             = user.Email!,
            FirstName         = user.FirstName,
            LastName          = user.LastName,
            PhoneNumber       = user.PhoneNumber ?? string.Empty,
            IsActive          = user.isActive,
            EmailConfirmed    = user.EmailConfirmed,
            AccessFailedCount = user.AccessFailedCount,
            LockoutEnd        = user.LockoutEnd,
            Roles             = roles.ToList()
        };
    }
}