using SmartApp.Application.DTOs.Auth;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApp.Application.Interfaces.Auth
{
    public interface ITokenService
    {
        Task<AuthResponseDto> GenerateAuthResponseAsync(
    ApplicationUser user,
    DeviceFingerprint fingerprint,
    CancellationToken cancellationToken = default);

        string GenerateAccessToken(ApplicationUser user, IList<string> roles);

    }
}
