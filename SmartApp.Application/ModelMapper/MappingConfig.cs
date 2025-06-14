using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApp.Application.ModelMapper
{
    using AutoMapper;
    using NBSoft.Application.ViewModel.Campus;
    using NBSoft.Application.ViewModel.HRM;
    using NBSoft.Domain.Model.Campus;
    using NBSoft.Domain.Model.HRM;
    using SmartApp.Domain.Entities.MasterData;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static System.Runtime.InteropServices.JavaScript.JSType;

    namespace NBSoft.Application.ModelMapper
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
}
