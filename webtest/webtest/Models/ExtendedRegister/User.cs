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
        [Required(AllowEmptyStrings =false, ErrorMessage ="Please insert your first name")]
        public string Name { get; set; }

        [Display (Name = "Last Name")]
        [Required(AllowEmptyStrings =false, ErrorMessage ="Please insert your surname")]
        public string Surname { get; set; }

        [Display(Name ="Email")]
        [Required(AllowEmptyStrings =false,ErrorMessage ="Please insert your email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(AllowEmptyStrings =false,ErrorMessage ="Please insert your Password")]
        [DataType(DataType.Password)]
        [MinLength(6,ErrorMessage ="Password must be at least 6 charachters")]
        [RegularExpression("^.*(?=.{6,})(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).*$", ErrorMessage ="Your Password must contain at least one capital letter, one number and one small letter")]
        public string Password { get; set; }

        [Display(Name= "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="The passwords do not match")]
        public string ConfirmPassword { get; set; }

        [Display(Name="Phone Number")]
        [Required(AllowEmptyStrings =false,ErrorMessage ="Please insert your Phone Number")]
        public string Phone_number { get; set; }
    }

}