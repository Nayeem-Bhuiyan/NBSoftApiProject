using SmartApp.Application.Interfaces.MasterData;
using SmartApp.Application.Interfaces.Repositories;
using SmartApp.Domain.Entities.MasterData;
using SmartApp.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApp.Infrastructure.Services.MasterData
{

    public class CountryService:ICountryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CountryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<Country>> CreateAsync(Country emp)
        {
            var response = await _unitOfWork.Repository<Country>().AddAsync(emp);
            if (!response.Success) return response;

            await _unitOfWork.SaveChangesAsync();
            return response;
        }
    }

}
