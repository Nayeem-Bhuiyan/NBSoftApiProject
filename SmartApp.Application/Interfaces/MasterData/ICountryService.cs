using SmartApp.Application.DTOs.MasterData.Country;
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
        Task<Response<Country>> CreateAsync(CreateCountryDto countryDto);
        Task<Response<Country>> UpdateAsync(UpdateCountryDto countryDto);
        Task<Response<bool>> DeleteAsync(object id);
        Task<Response<CountryDto>> GetByIdAsync(object id);
        Task<Response<PagedResult<CountryDto>>> GetPagedAsync(string? filter, int pageIndex, int pageSize);
    }
}
