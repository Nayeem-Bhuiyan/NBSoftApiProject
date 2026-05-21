using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartApp.Application.DTOs.Auth;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.RoleManagement.Queries.GetAllRoles;

public sealed class GetAllRolesHandler : IRequestHandler<GetAllRolesQuery, Response<List<RoleDto>>>
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IMapper _mapper;

    public GetAllRolesHandler(RoleManager<ApplicationRole> roleManager, IMapper mapper)
    {
        _roleManager = roleManager;
        _mapper      = mapper;
    }

    public async Task<Response<List<RoleDto>>> Handle(GetAllRolesQuery request, CancellationToken ct)
    {
        var roles = await _roleManager.Roles
            .AsNoTracking()
            .ToListAsync(ct);

        return Response<List<RoleDto>>.SuccessResponse(
            _mapper.Map<List<RoleDto>>(roles), "Roles retrieved successfully.");
    }
}