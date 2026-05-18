using System;
using System.Collections.Generic;
using System.Text;

namespace SmartApp.Domain.Entities.Auth
{
    public class RolePermission
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public int PermissionId { get; set; }
        public bool IsGranted { get; set; }

        public ApplicationRole Role { get; set; }
        public Permission Permission { get; set; }
    }
}
