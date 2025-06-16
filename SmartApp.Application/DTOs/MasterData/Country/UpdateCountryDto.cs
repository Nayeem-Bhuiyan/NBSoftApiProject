using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApp.Application.DTOs.MasterData.Country
{
    public class UpdateCountryDto : CreateCountryDto
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}
