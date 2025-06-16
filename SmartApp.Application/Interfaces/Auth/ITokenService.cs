using SmartApp.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApp.Application.Interfaces.Auth
{
    public interface ITokenService
    {
        string GenerateToken(ApplicationUser user);
    }
}
