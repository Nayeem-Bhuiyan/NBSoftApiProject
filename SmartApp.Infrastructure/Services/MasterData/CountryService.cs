using SmartApp.Application.Interfaces.MasterData;
using SmartApp.Application.Interfaces.Repositories;
using SmartApp.Domain.Entities.MasterData;
using SmartApp.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SmartApp.Infrastructure.Services.MasterData
{
    public class CountryService : ICountryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CountryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<Country>> CreateAsync(Country country)
        {
            var response = await _unitOfWork.Repository<Country>().AddAsync(country);
            if (!response.isSuccess) return response;

            var saveResult = await _unitOfWork.SaveChangesAsync();
            if (!saveResult.isSuccess)
                return Response<Country>.Failure(saveResult.message);

            return response;
        }

        public async Task<Response<Country>> UpdateAsync(Country country)
        {
            var response = await _unitOfWork.Repository<Country>().UpdateAsync(country);
            if (!response.isSuccess) return response;

            var saveResult = await _unitOfWork.SaveChangesAsync();
            if (!saveResult.isSuccess)
                return Response<Country>.Failure(saveResult.message);

            return response;
        }

        public async Task<Response<bool>> DeleteAsync(object id)
        {
            var response = await _unitOfWork.Repository<Country>().DeleteAsync(id);
            if (!response.isSuccess) return response;

            var saveResult = await _unitOfWork.SaveChangesAsync();
            if (!saveResult.isSuccess)
                return Response<bool>.Failure(saveResult.message);

            return response;
        }

        public async Task<Response<Country>> GetByIdAsync(object id)
        {
            return await _unitOfWork.Repository<Country>().GetByIdAsync(id);
        }

        public async Task<Response<PagedResult<Country>>> GetPagedAsync(string? filter, int pageIndex, int pageSize)
        {
            Expression<Func<Country, bool>>? predicate = null;

            if (!string.IsNullOrWhiteSpace(filter))
                predicate = c => c.Name.Contains(filter);

            return await _unitOfWork.Repository<Country>().GetPagedAsync(predicate, pageIndex, pageSize);
        }
    }

    

}
