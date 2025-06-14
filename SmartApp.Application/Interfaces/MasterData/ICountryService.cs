using SmartApp.Domain.Entities.MasterData;
using SmartApp.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApp.Application.Interfaces.MasterData
{
    public interface ICountryService
    {
        Task<Response<Country>> CreateAsync(Country emp);
    }
}
