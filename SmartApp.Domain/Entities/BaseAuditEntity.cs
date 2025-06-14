using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartApp.Domain.Entities
{

        public abstract class BaseAuditEntity
        {
            public BaseAuditEntity()
            {
                if (this.Id>0)
                    updatedDate = DateTime.Now;
                else
                    this.createdDate = DateTime.Now;
            }


            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Key]
            public int Id { get; set; }

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
