using AutoMapper;
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

               // CreateMap<Country, Vm_StudentInfo>()
               //.ForMember(vm => vm.StudentInfoId, m => m.MapFrom(src => src.Id));
               // CreateMap<Vm_StudentInfo, StudentInfo>();
                #endregion
            }
        }
    
}
