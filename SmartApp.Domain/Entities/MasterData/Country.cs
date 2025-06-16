using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApp.Domain.Entities.MasterData
{
    public class Country: BaseAuditEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Continent { get; set; }
        public string Currency { get; set; }
        public string PhoneCode { get; set; }
        public bool IsActive { get; set; } = true;

    }
    
}
