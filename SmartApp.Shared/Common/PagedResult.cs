using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApp.Shared.Common
{
    public class PagedResult<T>
    {
        public IEnumerable<T> items { get; set; } = Enumerable.Empty<T>();
        public int totalCount { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public int totalPages => (int)Math.Ceiling((double)totalCount / pageSize);
    }
}
