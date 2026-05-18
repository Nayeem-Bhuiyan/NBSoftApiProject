using System;
using System.Collections.Generic;
using System.Text;

namespace SmartApp.Domain.Entities.Auth
{
    public class Permission
    {
        public int Id { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string DisplayName { get; set; }  // e.g. "Country - Delete"
        public string HttpMethod { get; set; }   // GET, POST, PUT, DELETE

        public ICollection<RolePermission> RolePermissions { get; set; }
    }
}
