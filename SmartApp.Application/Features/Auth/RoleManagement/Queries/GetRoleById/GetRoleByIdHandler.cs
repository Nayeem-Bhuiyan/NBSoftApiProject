using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SmartApp.Application.DTOs.Auth;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.RoleManagement.Queries.GetRoleById;

public sealed class GetRoleByIdHandler : IRequestHandler<GetRoleByIdQuery, Response<RoleDto>>
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IMapper _mapper;

    public GetRoleByIdHandler(RoleManager<ApplicationRole> roleManager, IMapper mapper)
    {
        _roleManager = roleManager;
        _mapper      = mapper;
    }

    public async Task<Response<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken ct)
    {
        var role = await _roleManager.FindByIdAsync(request.Id);
        if (role is null)
            return Response<RoleDto>.Failure("Role not found.");

        return Response<RoleDto>.SuccessResponse(_mapper.Map<RoleDto>(role), "Role retrieved successfully.");
    }
}