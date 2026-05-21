using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SmartApp.Application.DTOs.Auth;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.RoleManagement.Commands.UpdateRole;

public sealed class UpdateRoleHandler : IRequestHandler<UpdateRoleCommand, Response<RoleDto>>
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IMapper _mapper;

    public UpdateRoleHandler(RoleManager<ApplicationRole> roleManager, IMapper mapper)
    {
        _roleManager = roleManager;
        _mapper      = mapper;
    }

    public async Task<Response<RoleDto>> Handle(UpdateRoleCommand request, CancellationToken ct)
    {
        var role = await _roleManager.FindByIdAsync(request.Id);
        if (role is null)
            return Response<RoleDto>.Failure("Role not found.");

        role.Name        = request.Name;
        role.Description = request.Description;

        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
            return Response<RoleDto>.Failure(string.Join("; ", result.Errors.Select(e => e.Description)));

        return Response<RoleDto>.SuccessResponse(_mapper.Map<RoleDto>(role), "Role updated successfully.");
    }
}