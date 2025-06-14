using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApp.Application.DTOs.Common
{
    public class DropdownItemDto<T>
    {
        public T Value { get; set; }
        public string Text { get; set; }
        public bool Selected { get; set; }
    }
}
