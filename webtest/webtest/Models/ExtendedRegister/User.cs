using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace webtest.Models
{
    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        public string ConfirmPassword { get; set; }
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

        [Required(AllowEmptyStrings =false,ErrorMessage ="Please insert hier your Password")]
        [DataType(DataType.Password)]
        [MinLength(6,ErrorMessage ="Password must be at least 6 charachters")]
        public string Password { get; set; }

        [Display(Name= "Confirm PAssword")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="your Confirm Password does not match your Password ")]
        public string ConfirmPassword { get; set; }

        [Display(Name="Phone Number")]
        [Required(AllowEmptyStrings =false,ErrorMessage ="Please insert hier your Phone Number")]
        public string Phone_number { get; set; }
    }

}