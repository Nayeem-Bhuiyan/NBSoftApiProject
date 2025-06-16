using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApp.Application.DTOs.MasterData.Country
{
    public class CreateCountryDto
    {
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string Continent { get; set; } = default!;
        public string Currency { get; set; } = default!;
        public string PhoneCode { get; set; } = default!;
    }
}
