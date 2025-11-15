using AutoMapper;
using SmartApp.Application.DTOs.MasterData.Country;
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
        private readonly IMapper _mapper;
        public CountryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper=mapper;
        }

        public async Task<Response<Country>> CreateAsync(CreateCountryDto createDto)
        {
            var createObj = _mapper.Map<Country>(createDto);
            var response = await _unitOfWork.Repository<Country>().AddAsync(createObj);
            if (!response.isSuccess) return response;

            var saveResult = await _unitOfWork.SaveChangesAsync();
            if (!saveResult.isSuccess)
                return Response<Country>.Failure(saveResult.message);

            return response;
        }

        public async Task<Response<Country>> UpdateAsync(UpdateCountryDto updateDto)
        {
            var updateObj = _mapper.Map<Country>(updateDto);
            var response = await _unitOfWork.Repository<Country>().UpdateAsync(updateObj);
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

        public async Task<Response<CountryDto>> GetByIdAsync(object id)
        {
            var countryResult = await _unitOfWork.Repository<Country>().GetByIdAsync(id);

            if (!countryResult.isSuccess || countryResult.data == null)
                return Response<CountryDto>.Failure(countryResult.message);

            var countryDto = _mapper.Map<CountryDto>(countryResult.data);
            return Response<CountryDto>.SuccessResponse(countryDto, "Country found");
        }


        public async Task<Response<PagedResult<CountryDto>>> GetPagedAsync(string filter, int pageIndex, int pageSize)
        {
            Expression<Func<Country, bool>> predicate = null;

            if (!string.IsNullOrWhiteSpace(filter))
                predicate = c => c.Name.Contains(filter);

            var result = await _unitOfWork.Repository<Country>().GetPagedAsync(predicate, pageIndex, pageSize);

            if (!result.isSuccess || result.data == null)
                return Response<PagedResult<CountryDto>>.Failure(result.message);

            var dtoItems = _mapper.Map<List<CountryDto>>(result.data.items);

            var pagedDtoResult = new PagedResult<CountryDto>
            {
                items = dtoItems,
                totalCount = result.data.totalCount,
                pageIndex = result.data.pageIndex,
                pageSize = result.data.pageSize
            };

            return Response<PagedResult<CountryDto>>.SuccessResponse(pagedDtoResult, "Paged data loaded successfully");
        }




    }



}
