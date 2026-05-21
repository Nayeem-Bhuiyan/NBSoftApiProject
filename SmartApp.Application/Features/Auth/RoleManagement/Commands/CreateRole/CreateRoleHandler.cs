using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SmartApp.Application.DTOs.Auth;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.RoleManagement.Commands.CreateRole;

public sealed class CreateRoleHandler : IRequestHandler<CreateRoleCommand, Response<RoleDto>>
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IMapper _mapper;

    public CreateRoleHandler(RoleManager<ApplicationRole> roleManager, IMapper mapper)
    {
        _roleManager = roleManager;
        _mapper      = mapper;
    }

    public async Task<Response<RoleDto>> Handle(CreateRoleCommand request, CancellationToken ct)
    {
        if (await _roleManager.RoleExistsAsync(request.Name))
            return Response<RoleDto>.Failure($"Role '{request.Name}' already exists.");

        var role = new ApplicationRole
        {
            Name        = request.Name,
            Description = request.Description
        };

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
            return Response<RoleDto>.Failure(string.Join("; ", result.Errors.Select(e => e.Description)));

        return Response<RoleDto>.SuccessResponse(_mapper.Map<RoleDto>(role), "Role created successfully.");
    }
}