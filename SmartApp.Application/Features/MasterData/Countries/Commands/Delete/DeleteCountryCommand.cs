using MediatR;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.MasterData.Countries.Commands.DeleteCountry;

public sealed record DeleteCountryCommand(int Id) : IRequest<Response<bool>>;