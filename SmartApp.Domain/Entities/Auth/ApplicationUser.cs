using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace SmartApp.Domain.Entities.Auth
{
    public class ApplicationUser : IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName
        {
            get { return FirstName + " " + LastName; }
        }
        

        [DefaultValue(true)]
        public bool? isActive { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? LastLoginTime { get; set; }
        public bool IsLockedOut => LockoutEnabled && LockoutEnd >= DateTimeOffset.Now;

       

    }
}
