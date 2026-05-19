using AutoMapper;
using SmartApp.Application.DTOs.Auth;
using SmartApp.Application.DTOs.MasterData.Country;
using SmartApp.Application.Features.MasterData.Countries.Commands.CreateCountry;
using SmartApp.Application.Features.MasterData.Countries.Commands.UpdateCountry;
using SmartApp.Domain.Entities.Auth;
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
            CreateMap<CreateCountryCommand, Country>();
            CreateMap<UpdateCountryCommand, Country>();
            CreateMap<Country, CountryDto>();
            #endregion

            #region Auth
            CreateMap<ApplicationUser, RegisterDto>().ReverseMap();
                    CreateMap<ApplicationUser, ApplicationUserDto>().ReverseMap();

            #endregion
        }
    }
    
}
