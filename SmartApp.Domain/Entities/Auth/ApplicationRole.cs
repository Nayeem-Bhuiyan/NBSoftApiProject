using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SmartApp.Domain.Entities.Auth
{

        public class ApplicationRole : IdentityRole
        {
            public ApplicationRole() : base() { }
            public ApplicationRole(string roleName) : base(roleName)
            {
            }
            //public string code { get; set; }
            public string Description { get; set; }
            public string IpAddress { get; set; }

            #region Audit_Trail_Properties
            [StringLength(120)]
            public string createdBy { get; set; }
            [DataType(DataType.DateTime)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
            public DateTime? createdDate { get; set; }
            [StringLength(120)]
            public string updatedBy { get; set; }
            [DataType(DataType.DateTime)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
            public DateTime? updatedDate { get; set; }

            [DefaultValue(false)]
            public bool? isDeleted { get; set; }
            [StringLength(120)]
            public string deletedBy { get; set; }
            [DataType(DataType.DateTime)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
            public DateTime? deletedDate { get; set; }
            #endregion


        }
    
}
