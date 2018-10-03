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
        [RegularExpression("[A-Z][a-z]*", ErrorMessage = "Your name cannot contain any numbers or characters. Please start the name with a capital letter.")]
        public string Name { get; set; }

        [Display (Name = "Last Name")]
        [Required(AllowEmptyStrings =false, ErrorMessage ="Please insert your surname")]
        [RegularExpression("[A-Z][a-z]*", ErrorMessage = "Your surname cannot contain any numbers or characters. Please start the name with a capital letter.")]
        public string Surname { get; set; }

        [Display(Name ="Email")]
        [Required(AllowEmptyStrings =false,ErrorMessage ="Please insert your email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(AllowEmptyStrings =false,ErrorMessage ="Please insert your password")]
        [DataType(DataType.Password)]
        [MinLength(6,ErrorMessage ="Password must be at least 6 charachters")]
        [RegularExpression("^.*(?=.{6,})(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).*$", ErrorMessage ="Your password must contain at least one capital letter, one number and one small letter")]
        public string Password { get; set; }

        [Display(Name= "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="The passwords do not match")]
        public string ConfirmPassword { get; set; }

        [Display(Name="Phone Number")]
        [Required(AllowEmptyStrings =true,ErrorMessage ="Please insert your phone number")]
        [RegularExpression("([0-9]{10})", ErrorMessage = "Please enter a valid phone number. Please start with 06.")]
        public string Phone_number { get; set; }
    }

}