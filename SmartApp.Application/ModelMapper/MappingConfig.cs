using AutoMapper;
using SmartApp.Application.DTOs.MasterData.Country;
using SmartApp.Domain.Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApp.Application.ModelMapper
{

        public class MappingConfig : Profile
        {
            public MappingConfig()
            {

                    #region MasterData
                    CreateMap<Country, CountryDto>();
                    CreateMap<CreateCountryDto, Country>();
                    CreateMap<UpdateCountryDto, Country>();
                    #endregion
            }
        }
    
}
