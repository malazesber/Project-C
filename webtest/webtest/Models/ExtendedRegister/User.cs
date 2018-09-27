using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace webtest.Models
{
    public partial class User
    {
        
    }

    public class UserMetadata
    {
        [Display(Name = "First Name")]
        [Required(AllowEmptyStrings =false, ErrorMessage ="Please insert hier your first name")]
        public string Name { get; set; }

        [Display (Name = "Last Name")]
        [Required(AllowEmptyStrings =false, ErrorMessage ="Please insert hier your Last Name")]
        public string Surname { get; set; }

        [Display(Name ="Email")]
        [Required(AllowEmptyStrings =false,ErrorMessage ="Please insert hier your email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


    }

}